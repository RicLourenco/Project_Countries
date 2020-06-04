namespace WPF_Project_Countries.Services
{
    #region Usings

    using System;
    using System.Collections.Generic;
    using System.Data.SQLite;
    using System.Threading.Tasks;
    using Library.Models;

    #endregion

    class AltSpellingsService
    {
        #region Variables

        private SQLiteCommand command;

        private readonly DialogService dialogService = new DialogService();

        #endregion

        #region Constructor

        /// <summary>
        /// Parametrized constructor for the alterate spellings services, that receives an SQLiteConnection and creates an SQLite table for the AltSpelling string list
        /// </summary>
        /// <param name="connection"></param>
        public AltSpellingsService(SQLiteConnection connection)
        {
            try
            {

                string sqlcommand = "create table if not exists altSpellings(alpha3code char(3), altSpelling varchar(50), foreign key (alpha3code) references countries(alpha3code));";

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
        /// Gets the altSpelling strings from the sqlite table altSpellings and adds them to a country's altSpellings list
        /// </summary>
        /// <param name="country"></param>
        /// <param name="connection"></param>
        /// <returns></returns>
        public async Task GetAltSpellings(Country country, SQLiteConnection connection)
        {
            try
            {
                await Task.Run(() =>
                {
                    string sql = $"select alpha3code, altSpelling from altSpellings where alpha3code = '{country.Alpha3Code}'";

                    command = new SQLiteCommand(sql, connection);

                    SQLiteDataReader reader = command.ExecuteReader();

                    country.AltSpellings = new List<string>();

                    while (reader.Read())
                    {
                        country.AltSpellings.Add(reader["altSpelling"].ToString());
                    }
                });
            }
            catch (Exception e)
            {
                dialogService.ShowMessage("Error", e.Message);
            }
        }

        /// <summary>
        /// Receives a country's alpha3code and its respective list of altSpellings string, and inserts it into the altSpellings sqlite table using an SQLiteConnection
        /// </summary>
        /// <param name="altSpellings"></param>
        /// <param name="alpha3code"></param>
        /// <param name="connection"></param>
        /// <returns></returns>
        public async Task SaveAltSpellingsAsync(List<string> altSpellings, string alpha3code, SQLiteConnection connection)
        {
            try
            {
                foreach (string altSpelling in altSpellings)
                {
                    command.Parameters.AddWithValue("@alpha3code", alpha3code);
                    command.Parameters.AddWithValue("@altSpelling", altSpelling);

                    command.CommandText = "insert into altSpellings values(@alpha3code, @altSpelling)";

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