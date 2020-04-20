using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
using Library.Services;

namespace WPF_Project_Countries
{
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
        }

        private void LoadLocalRates()
        {
            throw new NotImplementedException();
        }

        private Task LoadApiRates()
        {
            throw new NotImplementedException();
        }
    }
}
