namespace WPF_Project_Countries
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.PerformanceData;
    using System.Drawing;
    using System.IO;
    using System.IO.Packaging;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Web.UI.WebControls;
    using System.Web.WebSockets;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Documents;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using System.Windows.Navigation;
    using System.Windows.Shapes;
    using Library.Models;
    using WPF_Project_Countries.Services;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<Country> Countries;

        private NetworkService networkService;

        private ApiService apiService;

        private DialogService dialogService;

        private DataService dataService;

        public Country country;

        public MainWindow()
        {
            InitializeComponent();
            networkService = new NetworkService();
            apiService = new ApiService();
            dialogService = new DialogService();
            dataService = new DataService();
            Countries = new List<Country>();
            country = new Country();
            LoadCountries();
        }

        private async void LoadCountries()
        {
            var connection = networkService.CheckConnection();

            if (connection.IsSuccess)
            {
                await LoadApiRates();
            }
            else
            {
                LoadLocalRates();
                Label_status.Visibility = Visibility.Visible;
                Label_status.Content = "Countries loaded from the offline database";
            }

            if(Countries == null)
            {
                Label_status.Content = "The countries couldn't be loaded\nbecause the program didn't finish saving\nbefore closing last time it connected to the internet";
            }
            else if(Countries.Count == 0)
            {
                ComboBox_countries.IsEnabled = false;
                Label_status.Content = "For the initial setup, an internet connection is needed\nPlease restart the program after connecting to the internet";
                return;
            }

            ComboBox_countries.ItemsSource = Countries;
            ComboBox_countries.DisplayMemberPath = "Name";
            ComboBox_countries.SelectedIndex = 0;

            Button_details.IsEnabled = true;
        }

        private void ReportProgress(object sender, ProgressReport e)
        {
            ProgressBar_api.Value = e.CompletePercentage;
            if(e.CompletePercentage != 100)
            {
                LabelApiProgress.Content = $"Loading... {e.CompletePercentage}%";
            }
            else
            {
                LabelApiProgress.Content = $"Countries loaded from the API on {DateTime.UtcNow}";
            }
            
        }

        private void DatabaseReportProgress(object sender, ProgressReport e)
        {
            ProgressBar_database.Value = e.CompletePercentage;
            LabelDatabaseProgress.Content = $"{e.CompletePercentage}%";
            if(e.CompletePercentage != 100)
            {
                LabelDatabaseProgress.Content = $"Saving... {e.CompletePercentage}%";
            }
            else
            {
                LabelDatabaseProgress.Content = "Countries saved to the database";
            }
        }

        private void LoadLocalRates()
        {
            Countries = dataService.GetData();
        }

        private async Task LoadApiRates()
        {
            ProgressBar_api.Visibility = Visibility.Visible;

            Progress<ProgressReport> apiProgress = new Progress<ProgressReport>();
            apiProgress.ProgressChanged += ReportProgress;

            var response = await apiService.GetCountries("http://restcountries.eu", "/rest/v2/all", apiProgress);

            Countries = (List<Country>) response.Result;

            dataService.DeleteData();

            Progress<ProgressReport> databaseProgress = new Progress<ProgressReport>();
            databaseProgress.ProgressChanged += DatabaseReportProgress;

            ProgressBar_database.Visibility = Visibility.Visible;

            dataService.SaveData(Countries, databaseProgress);
        }

        private void ComboBox_countries_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            country = (Country) ComboBox_countries.SelectedItem;

            TextBox_capital.Text = CheckEmptyStrings(country.Capital);
            TextBox_gini.Text = CheckEmptyStrings(country.Gini.ToString());
            TextBox_population.Text = CheckEmptyStrings(country.Population.ToString());
            TextBox_region.Text = CheckEmptyStrings(country.Region);
            TextBox_subregion.Text = CheckEmptyStrings(country.Subregion);
            Image_flag.Source = dataService.GetFlag(country);
        }

        private void Button_close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Button_details_Click(object sender, RoutedEventArgs e)
        {
            DetailsWindow details = new DetailsWindow(country);
            details.Show();
        }

        private string CheckEmptyStrings(string field)
        {
            if (string.IsNullOrWhiteSpace(field))
            {
                return "N/A";
            }
            else if(field == "0")
            {
                return "N/A";
            }
            else
            {
                return field;
            }
        }
    }
}
