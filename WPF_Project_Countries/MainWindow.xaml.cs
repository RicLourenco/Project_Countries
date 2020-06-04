namespace WPF_Project_Countries
{
    #region Usings

    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using Library.Models;
    using WPF_Project_Countries.Services;

    #endregion

    public partial class MainWindow : Window
    {
        #region Variables

        public List<Country> countries;

        readonly NetworkService networkService;

        readonly ApiService apiService;

        readonly DataService dataService;

        readonly DialogService dialogService;

        public Country country;

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor that creates a new instance of every variable object contained in this window
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            networkService = new NetworkService();
            apiService = new ApiService();
            dataService = new DataService();
            countries = new List<Country>();
            country = new Country();
            dialogService = new DialogService();
            LoadCountries();
        }

        #endregion

        #region WPF related methods (alphabetical order)

        /// <summary>
        /// Closes program
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        /// <summary>
        /// Instances a new window to show all the detailed info from the country selected in the combo box
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_details_Click(object sender, RoutedEventArgs e)
        {
            DetailsWindow details = new DetailsWindow(this);
            details.Show();
            ComboBox_countries.IsEnabled = false;
            Button_details.IsEnabled = false;
        }

        /// <summary>
        /// Changes all textboxes and image based on the country selected in the combo box
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ComboBox_countries_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            country = (Country)ComboBox_countries.SelectedItem;

            TextBox_capital.Text = CheckEmptyStrings(country.Capital);
            TextBox_gini.Text = CheckEmptyStrings(country.Gini.ToString());
            TextBox_population.Text = CheckEmptyStrings(country.Population.ToString());
            TextBox_region.Text = CheckEmptyStrings(country.Region);
            TextBox_subregion.Text = CheckEmptyStrings(country.Subregion);
            Image_flag.Source = dataService.GetFlag(country);
        }

        /// <summary>
        /// Shuts down the application when this window is closing, or else the application will keep running if the user opens the details window, closes it, and then closes this window, even though it looks like the application closed. This happens because a new instance of a main window is created when opening the details window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Application.Current.Shutdown();
        }

        #endregion

        #region Other methods (alphabetical order)

        /// <summary>
        /// Checks a string and if it's null or empty, and if it is, replaces it with an appropriate substitute
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
        private string CheckEmptyStrings(string field)
        {
            if (string.IsNullOrWhiteSpace(field))
            {
                return "N/A";
            }
            else if (field == "0")
            {
                return "N/A";
            }
            else
            {
                return field;
            }
        }

        /// <summary>
        /// Increments the database progress bar and its respective label based on a received value between 0 and 100, while it's saving all data to the database
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DatabaseReportProgress(object sender, ProgressReport e)
        {
            ProgressBar_database.Value = e.CompletePercentage;
            LabelDatabaseProgress.Content = $"{e.CompletePercentage}%";
            if (e.CompletePercentage != 100)
            {
                LabelDatabaseProgress.Content = $"Saving... {e.CompletePercentage}%";
            }
            else
            {
                LabelDatabaseProgress.Content = "Countries saved to the database";
            }
        }

        /// <summary>
        /// Loads the countries list from the API by providing the appropriate link, and saves all data to the database afterwards
        /// </summary>
        /// <returns></returns>
        private async Task LoadApiCountries()
        {
            dataService.DeleteData();

            ProgressBar_api.Visibility = Visibility.Visible;

            Progress<ProgressReport> apiProgress = new Progress<ProgressReport>();
            apiProgress.ProgressChanged += ReportProgress;

            var response = await apiService.GetCountries("http://restcountries.eu", "/rest/v2/all", apiProgress);

            if (response.IsSuccess == false)
            {
                dialogService.ShowMessage("Error", "The countries couldn't be loaded\nmost likely the API no longer exists");
                Close();
                return;
            }

            countries = (List<Country>)response.Result;

            Progress<ProgressReport> databaseProgress = new Progress<ProgressReport>();
            databaseProgress.ProgressChanged += DatabaseReportProgress;

            ProgressBar_database.Visibility = Visibility.Visible;

            dataService.SaveData(countries, databaseProgress);
        }

        /// <summary>
        /// Main method for loading all countries, wether from the API of the offline database
        /// </summary>
        private async void LoadCountries()
        {
            var connection = networkService.CheckConnection();

            if (connection.IsSuccess)
            {
                await LoadApiCountries();
            }
            else
            {
                await LoadLocalCountries();
                Label_status.Visibility = Visibility.Visible;
                Label_status.Content = "Countries loaded from the offline database";
            }

            ComboBox_countries.IsEnabled = true;
            Button_details.IsEnabled = true;

            if (countries == null)
            {
                Label_status.Foreground = new SolidColorBrush(Colors.Red);
                Label_status.Content = "The countries couldn't be loaded\nbecause the program didn't finish saving\nbefore closing last time\nit connected to the internet";
                ComboBox_countries.IsEnabled = false;
                Button_details.IsEnabled = false;
            }
            else if (countries.Count == 0)
            {
                Label_status.Foreground = new SolidColorBrush(Colors.Red);
                Label_status.Content = "For the initial setup\nan internet connection is needed\nPlease restart the program\nafter connecting to the internet";
                ComboBox_countries.IsEnabled = false;
                Button_details.IsEnabled = false;
                return;
            }

            ComboBox_countries.ItemsSource = countries;
            ComboBox_countries.DisplayMemberPath = "Name";
            ComboBox_countries.SelectedIndex = 0;
        }

        /// <summary>
        /// Loads all countries from the database by calling the appropriate method from the data service
        /// </summary>
        /// <returns></returns>
        private async Task LoadLocalCountries()
        {
            countries = await dataService.GetData();
        }

        /// <summary>
        /// Increments the API progress bar and its respective label based on a received value between 0 and 100, while it's converting all the flags
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ReportProgress(object sender, ProgressReport e)
        {
            ProgressBar_api.Value = e.CompletePercentage;
            if (e.CompletePercentage != 100)
            {
                LabelApiProgress.Content = $"Loading... {e.CompletePercentage}%";
            }
            else
            {
                LabelApiProgress.Content = $"Countries loaded from the API on {DateTime.UtcNow:dd/MM/yyyy}";
            }

        }

        #endregion
    }
}