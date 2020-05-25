using Library.Models;
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
using System.Windows.Shapes;

namespace WPF_Project_Countries
{
    /// <summary>
    /// Interaction logic for DetailsWindow.xaml
    /// </summary>
    public partial class DetailsWindow : Window
    {
        Country country = new Country();

        public DetailsWindow(Country c)
        {
            InitializeComponent();
            country = c;
            FillFields();
        }

        private void FillFields()
        {
            ListBox_topLevelDomains.ItemsSource = country.TopLevelDomain;
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
            ListBox_altSpellings.ItemsSource = country.AltSpellings;
            ListBox_borders.ItemsSource = country.Borders;
            ListBox_callingCodes.ItemsSource = country.CallingCodes;
            ListBox_latlng.ItemsSource = country.Latlng;
            ListBox_timeZones.ItemsSource = country.Timezones;
            GridView_regionalBlocs.ItemsSource = country.RegionalBlocs;
            ListBox_otherAcronyms.ItemsSource = GetOtherAcronyms(country.RegionalBlocs);
            ListBox_otherNames.ItemsSource = GetOtherNames(country.RegionalBlocs);
        }

        private List<string> GetOtherAcronyms(List<RegionalBloc> regionalBlocs)
        {
            List<string> newList = new List<string>();
            newList.Add("Other acronyms:");

            foreach (RegionalBloc regionalBloc in regionalBlocs)
            {
                foreach (string otherAcronym in regionalBloc.OtherAcronyms)
                {
                    newList.Add(otherAcronym);
                }
            }

            if(newList.Count > 1)
            {
                return newList;
            }
            else
            {
                newList.Add("N/A");
                return newList;
            }
        }

        private List<string> GetOtherNames(List<RegionalBloc> regionalBlocs)
        {
            List<string> newList = new List<string>();
            newList.Add("Other names:");

            foreach (RegionalBloc regionalBloc in regionalBlocs)
            {
                foreach (string otherName in regionalBloc.OtherNames)
                {
                    newList.Add(otherName);
                }
            }

            if (newList.Count > 1)
            {
                return newList;
            }
            else
            {
                newList.Add("N/A");
                return newList;
            }
        }

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

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
