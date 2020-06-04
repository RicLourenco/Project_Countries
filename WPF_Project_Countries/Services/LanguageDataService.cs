namespace WPF_Project_Countries.Services
{
    #region Usings

    using System;
    using System.Collections.Generic;
    using System.Data.SQLite;
    using System.Threading.Tasks;
    using Library.Models;

    #endregion

    class LanguageDataService
    {
        #region Variables

        private SQLiteCommand command;

        private readonly DialogService dialogService = new DialogService();

        #endregion

        #region Constructor

        /// <summary>
        /// Parametrized constructor for the languages services, that receives an SQLiteConnection and creates an SQLite table for the Language object
        /// </summary>
        /// <param name="connection"></param>
        public LanguageDataService(SQLiteConnection connection)
        {
            try
            {
                string sqlcommand = "create table if not exists languages(alpha3code char(3), iso639_1 char(2), iso639_2 char(3), name varchar(50), nativeName varchar(50), foreign key(alpha3code) references country(alpha3code))";

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
        /// Gets the languages objects from the sqlite table languages and adds them to a country's languages list
        /// </summary>
        /// <param name="country"></param>
        /// <param name="connection"></param>
        public async Task GetLanguages(Country country, SQLiteConnection connection)
        {
            try
            {
                await Task.Run(() =>
                {
                    string sql = $"select alpha3code, iso639_1, iso639_2, name, nativeName from languages where alpha3code = '{country.Alpha3Code}'";

                    command = new SQLiteCommand(sql, connection);

                    SQLiteDataReader reader = command.ExecuteReader();

                    country.Languages = new List<Language>();

                    while (reader.Read())
                    {
                        country.Languages.Add(new Language
                        {
                            Iso639_1 = reader["iso639_1"].ToString(),
                            Iso639_2 = reader["iso639_2"].ToString(),
                            Name = reader["name"].ToString(),
                            NativeName = reader["nativeName"].ToString()
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
        /// Receives a country's alpha3code and its respective list of languages object, and inserts it into the languages sqlite table using an SQLiteConnection
        /// </summary>
        /// <param name="languages"></param>
        /// <param name="alpha3code"></param>
        /// <param name="connection"></param>
        /// <returns></returns>
        public async Task SaveLanguageAsync(List<Language> languages, string alpha3code, SQLiteConnection connection)
        {
            try
            {
                foreach (Language language in languages)
                {
                    command.Parameters.AddWithValue("@alpha3code", alpha3code);
                    command.Parameters.AddWithValue("@iso639_1", language.Iso639_1);
                    command.Parameters.AddWithValue("@iso639_2", language.Iso639_2);
                    command.Parameters.AddWithValue("@name", language.Name);
                    command.Parameters.AddWithValue("@nativeName", language.NativeName);

                    command.CommandText = "insert into languages values(@alpha3code, @iso639_1, @iso639_2, @name, @nativeName)";

                    command.Connection = connection;

                    await command.ExecuteNonQueryAsync();
                }
            }
            catch (Exception e)
            {
                dialogService.ShowMessage("Error", e.Message);
            }
        }

        #endregion
    }
}