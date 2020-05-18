namespace WPF_Project_Countries.Services
{
    using System;
    using System.Collections.Generic;
    using System.Data.SQLite;
    using System.IO;
    using System.Threading.Tasks;
    using System.Windows.Controls;
    using System.Windows.Media.Imaging;
    using Library.Models;

    public class DataService
    {
        private SQLiteConnection connection;
        private SQLiteCommand command;
        private DialogService dialogService;
        private AltSpellingsService altSpellings;
        private BordersService borders;
        private CallingCodesService callingCodes;
        private CurrencyDataService currency;
        private LanguageDataService language;
        private LatlngsService latlngs;
        private RegionalBlocDataService regionalBloc;
        private TimeZonesService timeZones;
        private TopLevelDomainService topLevelDomain;
        private TranslationsDataService translations;

        /// <summary>
        /// Default constructor that creates a new local sqlite database and its tables, if one doesn't exist
        /// </summary>
        public DataService()
        {
            dialogService = new DialogService();

            if (!Directory.Exists("Data"))
            {
                Directory.CreateDirectory("Data");
            }

            var path = @"Data\Countries.sqlite";

            try
            {
                connection = new SQLiteConnection("Data Source=" + path);
                connection.Open();

                string sqlcommand = "create table if not exists countries(name varchar(50), alpha2code char(2), alpha3code char(3) primary key, capital varchar(50), region varchar(50), subregion varchar(50), population integer, denonym varchar(50), area real, gini real, nativeName varchar(50), numericCode varchar(20), cioc varchar(20));";

                //CreateOtherTables();

                command = new SQLiteCommand(sqlcommand, connection);

                command.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                dialogService.ShowMessage("Erro", e.Message);
            }
        }

        private void CreateOtherTables()
        {
            altSpellings = new AltSpellingsService();
            borders = new BordersService();
            callingCodes = new CallingCodesService();
            currency = new CurrencyDataService();
            language = new LanguageDataService();
            latlngs = new LatlngsService();
            regionalBloc = new RegionalBlocDataService();
            timeZones = new TimeZonesService();
            topLevelDomain = new TopLevelDomainService();
            translations = new TranslationsDataService();
        }

        /// <summary>
        /// Receives a C# list of countries and inserts them into the countries sqlite table
        /// </summary>
        /// <param name="countries"></param>
        public async Task SaveData(List<Country> countries)
        {
            try
            {
                await Task.Run(async () => {
                    foreach (Country country in countries)
                    {
                        command.Parameters.AddWithValue("@name", country.Name);
                        command.Parameters.AddWithValue("@alpha2code", country.Alpha2Code);
                        command.Parameters.AddWithValue("@alpha3code", country.Alpha3Code);
                        command.Parameters.AddWithValue("@capital", country.Capital);
                        command.Parameters.AddWithValue("@region", country.Region);
                        command.Parameters.AddWithValue("@subregion", country.Subregion);
                        command.Parameters.AddWithValue("@population", country.Population);
                        command.Parameters.AddWithValue("@denonym", country.Demonym);
                        command.Parameters.AddWithValue("@area", country.Area);
                        command.Parameters.AddWithValue("@gini", country.Gini);
                        command.Parameters.AddWithValue("@nativeName", country.NativeName);
                        command.Parameters.AddWithValue("@numericCode", country.NumericCode);
                        command.Parameters.AddWithValue("@cioc", country.Cioc);

                        command.CommandText = "insert into countries values(@name, @alpha2code, @alpha3code, @capital, @region, @subregion, @population, @denonym, @area, @gini, @nativeName, @numericCode, @cioc)";

                        command.Connection = connection;

                        command.ExecuteNonQuery();

                        //await SaveDataToOtherTables(country);
                    }

                    connection.Close();
                });
            }
            catch (Exception e)
            {
                dialogService.ShowMessage("Erro", e.Message);
            }
        }

        private async Task SaveDataToOtherTables(Country country)
        {
            
            await altSpellings.SaveAltSpellings(country);
            await borders.SaveBordersAsync(country);
            await callingCodes.SaveCallingCodesAsync(country);
            await currency.SaveCurrencyAsync(country);
            await language.SaveLanguageAsync(country);
            await latlngs.SaveLatlngsAsync(country);
            //await regionalBloc.SaveRegionalBlocAsync(country);
            await timeZones.SaveTimeZonesAsync(country);
            await topLevelDomain.SaveTopLevelDomainAsync(country);
            await translations.SaveTranslationsAsync(country);
            
        }

        /// <summary>
        /// Reads all rows from the countries sqlite table and inserts them into the countries C# list
        /// </summary>
        /// <returns></returns>
        public List<Country> GetData()
        {
            List<Country> countries = new List<Country>();

            try
            {
                string sql = "select name, alpha2code, alpha3code, capital, region, subregion, population, denonym, area, gini, nativeName, numericCode, cioc from countries";

                command = new SQLiteCommand(sql, connection);

                SQLiteDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    countries.Add(new Country
                    {
                        Name = reader["name"].ToString(),
                        Alpha2Code = reader["alpha2code"].ToString(),
                        Alpha3Code = reader["alpha3code"].ToString(),
                        Capital = reader["capital"].ToString(),
                        Region = reader["region"].ToString(),
                        Subregion = reader["subregion"].ToString(),
                        Population = Convert.ToInt64(reader["population"]),
                        Demonym = reader["denonym"].ToString(),
                        Area = Convert.ToDouble(reader["area"]),
                        Gini = Convert.ToDouble(reader["gini"]),
                        NativeName = reader["nativeName"].ToString(),
                        NumericCode = reader["numericCode"].ToString(),
                        Cioc = reader["cioc"].ToString()
                    });
                }

                connection.Close();
                return countries;
            }
            catch (Exception e)
            {
                dialogService.ShowMessage("Erro", e.Message);
                return null;
            }
        }

        private void GetDataFromOtherTables()
        {

        }

        /// <summary>
        /// Deletes all rows from the contries sqlite table
        /// </summary>
        public void DeleteData()
        {
            try
            {
                string sql = "begin;" +
                    /*"delete from altSpellings;" +
                    "delete from borders;" +
                    "delete from callingCodes;" +
                    "delete from currencies;" +
                    "delete from languages;" +
                    "delete from latlngs;" +
                    "delete from otherAcronyms;" +
                    "delete from otherNames;" +
                    "delete from regionalBlocs;" +
                    "delete from timeZones;" +
                    "delete from topLevelDomains;" +
                    "delete from translations;" +*/
                    "delete from countries;" +
                    "commit;";

                command = new SQLiteCommand(sql, connection);

                command.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                dialogService.ShowMessage("Erro", e.Message);
            }
        }

        /// <summary>
        /// Gets a flag image from the Flags folder
        /// </summary>
        /// <param name="country"></param>
        /// <returns>BitmapImage with the appropriate flag in bmp format</returns>
        public BitmapImage GetFlag(Country country)
        {
            if (File.Exists($"{Environment.CurrentDirectory}\\Flags\\{country.Name}.bmp"))
            {
                return new BitmapImage(new Uri($"{Environment.CurrentDirectory}\\Flags\\{country.Name}.bmp", UriKind.RelativeOrAbsolute));
            }
            else
            {
                return new BitmapImage(new Uri($"{Environment.CurrentDirectory}\\Flags\\Default.bmp", UriKind.RelativeOrAbsolute));
            }
        }
    }
}
