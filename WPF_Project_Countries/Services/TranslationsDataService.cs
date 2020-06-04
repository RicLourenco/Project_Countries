namespace WPF_Project_Countries.Services
{
    #region Usings

    using System;
    using System.Data.SQLite;
    using System.Threading.Tasks;
    using Library.Models;

    #endregion

    class TranslationsDataService
    {
        #region Variables

        private SQLiteCommand command;

        private readonly DialogService dialogService = new DialogService();

        #endregion

        #region Constructor

        /// <summary>
        /// Parametrized constructor for the translations services, that receives an SQLiteConnection and creates an SQLite table for the translation object
        /// </summary>
        /// <param name="connection"></param>
        public TranslationsDataService(SQLiteConnection connection)
        {
            try
            {
                string sqlcommand = "create table if not exists translations(alpha3code char(3), de varchar(50), es varchar(50), fr varchar(50), ja varchar(50), it varchar(50), br varchar(50), pt varchar(50), nl varchar(50), hr varchar(50), fa varchar(50), foreign key(alpha3code) references country(alpha3code))";

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
        /// Gets the translations objects from the sqlite table translations and adds them to a country's translations
        /// </summary>
        /// <param name="country"></param>
        /// <param name="connection"></param>
        public async Task GetTranslations(Country country, SQLiteConnection connection)
        {
            try
            {
                await Task.Run(() =>
                {
                    string sql = $"select alpha3code, de, es, fr, ja, it, br, pt, nl, hr, fa from translations where alpha3code = '{country.Alpha3Code}'";

                    command = new SQLiteCommand(sql, connection);

                    SQLiteDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        country.Translations = (new Translations
                        {
                            De = reader["de"].ToString(),
                            Es = reader["es"].ToString(),
                            Fr = reader["fr"].ToString(),
                            Ja = reader["ja"].ToString(),
                            It = reader["it"].ToString(),
                            Br = reader["br"].ToString(),
                            Pt = reader["pt"].ToString(),
                            Nl = reader["nl"].ToString(),
                            Hr = reader["hr"].ToString(),
                            Fa = reader["fa"].ToString()
                        });
                    }
                });
            }
            catch (Exception e)
            {
                dialogService.ShowMessage("Error", e.Message);
            }
        }

        /// <summary>
        /// Receives a country's alpha3code and its respective translations object, and inserts it into the translations sqlite table using an SQLiteConnection
        /// </summary>
        /// <param name="translations"></param>
        /// <param name="alpha3code"></param>
        /// <param name="connection"></param>
        /// <returns></returns>
        public async Task SaveTranslationsAsync(Translations translations, string alpha3code, SQLiteConnection connection)
        {
            try
            {
                command.Parameters.AddWithValue("@alpha3code", alpha3code);
                command.Parameters.AddWithValue("@de", translations.De);
                command.Parameters.AddWithValue("@es", translations.Es);
                command.Parameters.AddWithValue("@fr", translations.Fr);
                command.Parameters.AddWithValue("@ja", translations.Ja);
                command.Parameters.AddWithValue("@it", translations.It);
                command.Parameters.AddWithValue("@br", translations.Br);
                command.Parameters.AddWithValue("@pt", translations.Pt);
                command.Parameters.AddWithValue("@nl", translations.Nl);
                command.Parameters.AddWithValue("@hr", translations.Hr);
                command.Parameters.AddWithValue("@fa", translations.Fa);

                command.CommandText = "insert into translations values(@alpha3code, @de, @es, @fr, @ja, @it, @br, @pt, @nl, @hr, @fa);";

                command.Connection = connection;

                await command.ExecuteNonQueryAsync();
            }
            catch (Exception e)
            {
                dialogService.ShowMessage("Error", e.Message);
            }
        }

        #endregion
    }
}