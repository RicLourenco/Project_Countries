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

                string sqlcommand = "create table if not exists countries(name varchar(50), alpha2code char(2), alpha3code char(3) primary key, capital varchar(50), region varchar(50), subregion varchar(50), population integer, denonym varchar(50), area real, gini real, nativeName varchar(50), numericCode varchar(20), cioc varchar(20));" +
                    "create table if not exists dbState(state boolean not null check(state in(0,1)));";

                CreateOtherTables();

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

                //await Task.Run(async () =>
                //{
                //   foreach (Country country in countries)
                //   {
                //       string sql = $"insert into countries values(\"{country.Name}\", '{country.Alpha2Code}', '{country.Alpha3Code}', \"{country.Capital}\", \"{country.Region}\", \"{country.Subregion}\", {country.Population}, \"{country.Demonym}\", '{country.Area}', '{country.Gini}', \"{country.NativeName}\", '{country.NumericCode}', '{country.Cioc}')";

                //       command = new SQLiteCommand(sql, connection);

                //       command.ExecuteNonQuery();

                //       await SaveDataToOtherTables(country);

                //       report.CompletePercentage = Convert.ToByte((i * 100) / countries.Count);
                //       progress.Report(report);
                //       i++;
                //   }
                //});

                    string sqlcommand = "update dbState set state = 1";

                    command = new SQLiteCommand(sqlcommand, connection);

                    command.ExecuteNonQuery();

                    connection.Close();
            }
            catch (Exception e)
            {
                dialogService.ShowMessage("Erro", e.Message);
            }
        }

        private async Task SaveDataToOtherTables(Country country)
        {

            await altSpellings.SaveAltSpellings(country.AltSpellings, country.Alpha3Code);
            await borders.SaveBordersAsync(country.Borders, country.Alpha3Code);
            await callingCodes.SaveCallingCodesAsync(country.CallingCodes, country.Alpha3Code);
            await currency.SaveCurrencyAsync(country.Currencies, country.Alpha3Code);
            await language.SaveLanguageAsync(country.Languages, country.Alpha3Code);
            await latlngs.SaveLatlngsAsync(country.Latlng, country.Alpha3Code);
            await regionalBloc.SaveRegionalBlocAsync(country.RegionalBlocs, country.Alpha3Code);
            await timeZones.SaveTimeZonesAsync(country.Timezones, country.Alpha3Code);
            await topLevelDomain.SaveTopLevelDomainAsync(country.TopLevelDomain, country.Alpha3Code);
            await translations.SaveTranslationsAsync(country.Translations, country.Alpha3Code);

        }

        /// <summary>
        /// Reads all rows from the countries sqlite table and inserts them into the countries C# list
        /// </summary>
        /// <returns></returns>
        public List<Country> GetData()
        {
            List<Country> countries = new List<Country>();

            string check = "select * from dbState";

            command = new SQLiteCommand(check, connection);

            SQLiteDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                if(Convert.ToByte(reader["state"]) == 0)
                {
                    return null;
                }
            }

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

                GetDataFromOtherTables(countries);

                connection.Close();

                return countries;
            }
            catch (Exception e)
            {
                dialogService.ShowMessage("Erro", e.Message);
                return null;
            }
        }

        private void GetDataFromOtherTables(List<Country> countries)
        {
            foreach (Country country in countries)
            {
                altSpellings.GetAltSpellings(country);
                borders.GetAltBorders(country);
                callingCodes.GetCallingCodes(country);
                currency.GetCurrencies(country);
                language.GetLanguages(country);
                latlngs.GetLatlngs(country);
                timeZones.GetTimeZones(country);
                topLevelDomain.GetTopLevelDomains(country);
                translations.GetTranslations(country);
                regionalBloc.GetRegionalBlocs(country);
            }
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
