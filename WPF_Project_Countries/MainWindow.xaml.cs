namespace WPF_Project_Countries
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.IO;
    using System.IO.Packaging;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Web.UI.WebControls;
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

        public MainWindow()
        {
            InitializeComponent();
            networkService = new NetworkService();
            apiService = new ApiService();
            dialogService = new DialogService();
            dataService = new DataService();
            LoadCountries();
        }

        private async void LoadCountries()
        {
            bool Load;

            var connection = networkService.CheckConnection();

            if (!connection.IsSuccess)
            {
                LoadLocalRates();
                Load = false;
            }
            else
            {
                await LoadApiRates();
                Load = true;
            }

            if(Countries.Count == 0)
            {
                this.IsEnabled = false;
                Label_status.Content = "Primeira";
                return;
            }

            ComboBox_countries.ItemsSource = Countries;
            ComboBox_countries.DisplayMemberPath = "Name";

            if(Load)
            {

            }
            else
            {

            }


        }

        private void LoadLocalRates()
        {
            Countries = dataService.GetData();
        }

        private async Task LoadApiRates()
        {
            var response = await apiService.GetCountries("http://restcountries.eu", "/rest/v2/all");

            Countries = (List<Country>) response.Result;
        }

        private void ComboBox_countries_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Country obj = (Country) ComboBox_countries.SelectedItem;

            TextBox_capital.Text = obj.Capital;
            TextBox_gini.Text = obj.Gini.ToString();
            TextBox_population.Text = obj.Population.ToString();
            TextBox_region.Text = obj.Region;
            TextBox_subregion.Text = obj.Subregion;
            Image_flag.Source = dataService.GetFlag(obj);
        }

        private void Button_close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
