namespace WPF_Project_Countries
{
    #region Usings

    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using Library.Models;

    #endregion

    public partial class DetailsWindow : Window
    {
        #region Variables

        Country country;

        readonly MainWindow main;

        #endregion

        #region Constructor

        /// <summary>
        /// Parametrized constructor that fills all fields and the countries list according to the received parameters
        /// </summary>
        /// <param name="c"></param>
        /// <param name="cs"></param>
        /// <param name="i"></param>
        /// <param name="m"></param>
        public DetailsWindow(MainWindow m)
        {
            InitializeComponent();
            main = m;
            ComboBoxSource();
            country = (Country) m.ComboBox_countries.SelectedItem;
        }

        #endregion

        #region WPF related methods (alphabetical order)

        /// <summary>
        /// Close button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        /// <summary>
        /// Besides changing the seleted country on this window, also changes the selected country on the main window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ComboBox_countries_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            country = (Country)ComboBox_countries.SelectedItem;
            main.ComboBox_countries.SelectedIndex = ComboBox_countries.SelectedIndex;
            FillFields();
        }

        /// <summary>
        /// Enables what is necessary on the main window, while this window is closing
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            main.Button_details.IsEnabled = true;
            main.ComboBox_countries.IsEnabled = true;
        }

        #endregion

        #region Other methods (alphabetical order)

        /// <summary>
        /// Concatenates the alpha 3 codes contained in a country's borders list with each corresponding country's name
        /// </summary>
        /// <param name="borders"></param>
        /// <returns></returns>
        private List<string> BorderNames(List<string> borders)
        {
            List<string> newList = new List<string>();

            foreach(string border in borders)
            {
                newList.Add($"{border}: {main.countries.FirstOrDefault(c => c.Alpha3Code == border).Name}");
            }

            return newList;
        }

        /// <summary>
        /// Receives a list of an undefined type, and checks if it's empty; if it is, returns a new string list with just "N/A"
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        private List<string> CheckEmptyLists<T>(List<T> list)
        {
            if (list.Count > 0)
            {
                List<string> newList = new List<string>();
                foreach (var i in list)
                {
                    newList.Add(i.ToString());
                }
                return newList;
            }
            else
            {
                List<string> newList = new List<string>
                {
                    "N/A"
                };
                return newList;
            }
        }

        /// <summary>
        /// Receives a string, and checks if it's empty; if it is, returns a new string with just "N/A"
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
        /// Fills this window's combo box with the countries brought from the main window
        /// </summary>
        private void ComboBoxSource()
        {
            ComboBox_countries.ItemsSource = main.countries;
            ComboBox_countries.DisplayMemberPath = "Name";
            ComboBox_countries.SelectedIndex = main.ComboBox_countries.SelectedIndex;
        }

        /// <summary>
        /// Fills all the fields with all the info from the selected country
        /// </summary>
        private void FillFields()
        {
            ListBox_topLevelDomains.ItemsSource = CheckEmptyLists(country.TopLevelDomain);
            TextBox_alpha2.Text = CheckEmptyStrings(country.Alpha2Code);
            TextBox_alpha3.Text = CheckEmptyStrings(country.Alpha3Code);
            TextBox_cioc.Text = CheckEmptyStrings(country.Cioc);
            TextBox_denonym.Text = CheckEmptyStrings(country.Demonym);
            TextBox_nativeName.Text = CheckEmptyStrings(country.NativeName);
            TextBox_numericCode.Text = CheckEmptyStrings(country.NumericCode);
            TextBox_br.Text = CheckEmptyStrings(country.Translations.Br);
            TextBox_de.Text = CheckEmptyStrings(country.Translations.De);
            TextBox_es.Text = CheckEmptyStrings(country.Translations.Es);
            TextBox_nl.Text = CheckEmptyStrings(country.Translations.Nl);
            TextBox_fa.Text = CheckEmptyStrings(country.Translations.Fa);
            TextBox_hr.Text = CheckEmptyStrings(country.Translations.Hr);
            TextBox_pt.Text = CheckEmptyStrings(country.Translations.Pt);
            TextBox_it.Text = CheckEmptyStrings(country.Translations.It);
            TextBox_ja.Text = CheckEmptyStrings(country.Translations.Ja);
            TextBox_fr.Text = CheckEmptyStrings(country.Translations.Fr);
            GridView_currencies.ItemsSource = country.Currencies;
            GridView_languages.ItemsSource = country.Languages;
            ListBox_altSpellings.ItemsSource = CheckEmptyLists(country.AltSpellings);
            ListBox_borders.ItemsSource = CheckEmptyLists(BorderNames(country.Borders));
            ListBox_callingCodes.ItemsSource = CheckEmptyLists(country.CallingCodes);
            ListBox_latlng.ItemsSource = CheckEmptyLists(country.Latlng);
            ListBox_timeZones.ItemsSource = CheckEmptyLists(country.Timezones);
            GridView_regionalBlocs.ItemsSource = country.RegionalBlocs;
            ListBox_otherAcronyms.ItemsSource = GetOtherAcronyms(country.RegionalBlocs);
            ListBox_otherNames.ItemsSource = GetOtherNames(country.RegionalBlocs);
        }

        /// <summary>
        /// Receives a list of regional blocs, and returns a concatenated list with all the other acronyms contained in every other acronym string list
        /// </summary>
        /// <param name="regionalBlocs"></param>
        /// <returns></returns>
        private List<string> GetOtherAcronyms(List<RegionalBloc> regionalBlocs)
        {
            List<string> newList = new List<string>();

            foreach (RegionalBloc regionalBloc in regionalBlocs)
            {
                foreach (string otherAcronym in regionalBloc.OtherAcronyms)
                {
                    newList.Add(otherAcronym);
                }
            }

            if (newList.Count > 0)
            {
                return newList;
            }

            newList.Add("N/A");

            return newList;
        }

        /// <summary>
        /// Receives a list of regional blocs, and returns a concatenated list with all the other names contained in every other name string list
        /// </summary>
        /// <param name="regionalBlocs"></param>
        /// <returns></returns>
        private List<string> GetOtherNames(List<RegionalBloc> regionalBlocs)
        {
            List<string> newList = new List<string>();

            foreach (RegionalBloc regionalBloc in regionalBlocs)
            {
                foreach (string otherName in regionalBloc.OtherNames)
                {
                    newList.Add(otherName);
                }
            }

            if (newList.Count > 0)
            {
                return newList;
            }

            newList.Add("N/A");

            return newList;
        }

        #endregion
    }
}