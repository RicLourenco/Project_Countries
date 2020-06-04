namespace WPF_Project_Countries.Services
{
    #region Usings

    using System;
    using System.Collections.Generic;
    using System.Data.SQLite;
    using System.IO;
    using System.Threading.Tasks;
    using System.Windows.Media.Imaging;
    using Library.Models;

    #endregion

    public class DataService
    {
        #region Variables
        private readonly SQLiteConnection connection;
        private SQLiteCommand command;
        private readonly DialogService dialogService = new DialogService();
        private SQLiteDataReader reader;
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
        #endregion

        #region Constructor

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

                string sqlcommand = "create table if not exists countries(name varchar(50), alpha2code char(2), alpha3code char(3) primary key, capital varchar(50), region varchar(50), subregion varchar(50), population integer, denonym varchar(50), area real, gini real, nativeName varchar(50), numericCode varchar(20), cioc varchar(20));" +
                    "create table if not exists dbState(state boolean not null check(state in(0,1)));";

                CreateOtherTables(connection);

                command = new SQLiteCommand(sqlcommand, connection);

                command.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                dialogService.ShowMessage("Error", e.Message);
            }
        }

        #endregion

        #region Methods (alphabetical order)

        /// <summary>
        /// Checks if the database finished saving last time the program was initialized, by checking the bool value in the sqlite table dbState
        /// </summary>
        /// <returns></returns>
        private bool CheckDataBaseState()
        {
            try
            {
                string check = "select * from dbState";

                command = new SQLiteCommand(check, connection);

                reader = command.ExecuteReader();

                while (reader.Read())
                {
                    if (Convert.ToByte(reader["state"]) == 0)
                    {
                        return false;
                    }
                }

                return true;
            }
            catch (Exception e)
            {
                dialogService.ShowMessage("Error", e.Message);
                return false;
            }
        }

        /// <summary>
        /// Calls all other services constructors to create a new sqlite table for each
        /// </summary>
        /// <param name="connection"></param>
        private void CreateOtherTables(SQLiteConnection connection)
        {
            altSpellings = new AltSpellingsService(connection);
            borders = new BordersService(connection);
            callingCodes = new CallingCodesService(connection);
            currency = new CurrencyDataService(connection);
            language = new LanguageDataService(connection);
            latlngs = new LatlngsService(connection);
            regionalBloc = new RegionalBlocDataService(connection);
            timeZones = new TimeZonesService(connection);
            topLevelDomain = new TopLevelDomainService(connection);
            translations = new TranslationsDataService(connection);
        }

        /// <summary>
        /// Deletes all rows from the contries sqlite table
        /// </summary>
        public void DeleteData()
        {
            try
            {
                string sql = "begin;" +
                    "delete from dbState;" +
                    "delete from altSpellings;" +
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
                    "delete from translations;" +
                    "delete from countries;" +
                    "insert into dbState(state) values(0);" +
                    "commit;";

                command = new SQLiteCommand(sql, connection);

                command.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                dialogService.ShowMessage("Error", e.Message);
            }
        }

        /// <summary>
        /// Reads all rows from the countries sqlite table and inserts them into the countries list
        /// </summary>
        /// <returns></returns>
        public async Task<List<Country>> GetData()
        {
            if (CheckDataBaseState() == false)
            {
                return null;
            }

            List<Country> countries = new List<Country>();

            try
            {
                string sql = "select name, alpha2code, alpha3code, capital, region, subregion, population, denonym, area, gini, nativeName, numericCode, cioc from countries";

                command = new SQLiteCommand(sql, connection);

                reader = command.ExecuteReader();

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

                await GetDataFromOtherTables(countries);

                connection.Close();

                return countries;
            }
            catch (Exception e)
            {
                dialogService.ShowMessage("Error", e.Message);
                return null;
            }
        }

        /// <summary>
        /// Calls the GetData methods from all other services
        /// </summary>
        /// <param name="countries"></param>
        /// <returns></returns>
        private async Task GetDataFromOtherTables(List<Country> countries)
        {
            foreach (Country country in countries)
            {
                await altSpellings.GetAltSpellings(country, connection);
                await borders.GetAltBorders(country, connection);
                await callingCodes.GetCallingCodes(country, connection);
                await currency.GetCurrencies(country, connection);
                await language.GetLanguages(country, connection);
                await latlngs.GetLatlngs(country, connection);
                await timeZones.GetTimeZones(country, connection);
                await topLevelDomain.GetTopLevelDomains(country, connection);
                await translations.GetTranslations(country, connection);
                await regionalBloc.GetRegionalBlocs(country, connection);
            }
        }

        /// <summary>
        /// Gets a country's bitmap flag from the appropriate directory, or the default flag if one doesn't exist
        /// </summary>
        /// <param name="country"></param>
        /// <returns></returns>
        public BitmapImage GetFlag(Country country)
        {
            try
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
            catch (Exception e)
            {
                dialogService.ShowMessage("Error", e.Message);
                return null;
            }
        }

        /// <summary>
        /// Receives a C# list of countries and inserts them into the countries sqlite table
        /// </summary>
        /// <param name="countries"></param>
        public async Task SaveData(List<Country> countries, IProgress<ProgressReport> progress)
        {
            ProgressReport report = new ProgressReport();
            byte i = 1;

            try
            {
                await Task.Run(async () =>
                {
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

                        await SaveDataToOtherTables(country);

                        report.CompletePercentage = Convert.ToByte((i * 100) / countries.Count);
                        progress.Report(report);
                        i++;
                    }
                });
                    string sqlcommand = "update dbState set state = 1";

                    command = new SQLiteCommand(sqlcommand, connection);

                    command.ExecuteNonQuery();

                    connection.Close();
            }
            catch (Exception e)
            {
                dialogService.ShowMessage("Error", e.Message);
            }
        }

        /// <summary>
        /// Calls all other save methods from other services, for every list and object property contained in any given country
        /// </summary>
        /// <param name="country"></param>
        /// <returns></returns>
        private async Task SaveDataToOtherTables(Country country)
        {

            await altSpellings.SaveAltSpellingsAsync(country.AltSpellings, country.Alpha3Code, connection);
            await borders.SaveBordersAsync(country.Borders, country.Alpha3Code, connection);
            await callingCodes.SaveCallingCodesAsync(country.CallingCodes, country.Alpha3Code, connection);
            await currency.SaveCurrencyAsync(country.Currencies, country.Alpha3Code, connection);
            await language.SaveLanguageAsync(country.Languages, country.Alpha3Code, connection);
            await latlngs.SaveLatlngsAsync(country.Latlng, country.Alpha3Code, connection);
            await regionalBloc.SaveRegionalBlocAsync(country.RegionalBlocs, country.Alpha3Code, connection);
            await timeZones.SaveTimeZonesAsync(country.Timezones, country.Alpha3Code, connection);
            await topLevelDomain.SaveTopLevelDomainAsync(country.TopLevelDomain, country.Alpha3Code, connection);
            await translations.SaveTranslationsAsync(country.Translations, country.Alpha3Code, connection);

        }

        #endregion
    }
}