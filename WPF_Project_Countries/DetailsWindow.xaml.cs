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
            TextBox_alpha2.Text = country.Alpha2Code;
            TextBox_alpha3.Text = country.Alpha3Code;
            TextBox_cioc.Text = country.Cioc;
            TextBox_denonym.Text = country.Demonym;
            TextBox_nativeName.Text = country.NativeName;
            TextBox_numericCode.Text = country.NumericCode;
            GridView_currencies.ItemsSource = country.Currencies;
            GridView_languages.ItemsSource = country.Languages;
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
