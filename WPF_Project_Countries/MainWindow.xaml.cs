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
            Label_status.Content = "Loading...";

            var connection = networkService.CheckConnection();

            if (connection.IsSuccess)
            {
                await LoadApiRates();
                Label_status.Content = $"Countries loaded from the API on {DateTime.UtcNow}";
            }
            else
            {
                LoadLocalRates();
                Label_status.Content = "Countries loaded from the local database";
            }

            if(Countries.Count == 0)
            {
                ComboBox_countries.IsEnabled = false;
                Label_status.Content = "For the initial setup, an internet connection is needed\nPlease restart the program after connecting to the internet";
                return;
            }

            ComboBox_countries.ItemsSource = Countries;
            ComboBox_countries.DisplayMemberPath = "Name";

            Button_details.IsEnabled = true;
        }

        private void ReportProgress(object sender, ProgressReport e)
        {
            ProgressBar_api.Value = e.CompletePercentage;
        }

        private void LoadLocalRates()
        {
            Countries = dataService.GetData();
        }

        private async Task LoadApiRates()
        {
            Progress<ProgressReport> apiProgress = new Progress<ProgressReport>();
            apiProgress.ProgressChanged += ReportProgress;

            var response = await apiService.GetCountries("http://restcountries.eu", "/rest/v2/all", apiProgress);

            Countries = (List<Country>) response.Result;

            dataService.DeleteData();

            dataService.SaveData(Countries);
        }

        private void ComboBox_countries_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            country = (Country) ComboBox_countries.SelectedItem;

            TextBox_capital.Text = country.Capital;
            TextBox_gini.Text = country.Gini.ToString();
            TextBox_population.Text = country.Population.ToString();
            TextBox_region.Text = country.Region;
            TextBox_subregion.Text = country.Subregion;
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
    }
}
