namespace WPF_Project_Countries.Services
{
    #region Usings

    using System;
    using System.Collections.Generic;
    using System.Data.SQLite;
    using System.Threading.Tasks;
    using Library.Models;

    #endregion

    class TimeZonesService
    {
        #region Variables

        private SQLiteCommand command;

        private readonly DialogService dialogService = new DialogService();

        #endregion

        #region Constructor

        /// <summary>
        /// Parametrized constructor for the time zones services, that receives an SQLiteConnection and creates an SQLite table for the TimeZone string list
        /// </summary>
        /// <param name="connection"></param>
        public TimeZonesService(SQLiteConnection connection)
        {
            try
            {
                string sqlcommand = "create table if not exists timeZones(alpha3code char(3), timeZone varchar(15), foreign key (alpha3code) references countries(alpha3code));";

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
        /// Gets the timeZones strings from the sqlite table timeZones and adds them to a country's timeZones list
        /// </summary>
        /// <param name="country"></param>
        /// <param name="connection"></param>
        public async Task GetTimeZones(Country country, SQLiteConnection connection)
        {
            try
            {
                await Task.Run(() =>
                {
                    string sql = $"select alpha3code, timeZone from timeZones where alpha3code = '{country.Alpha3Code}'";

                    command = new SQLiteCommand(sql, connection);

                    SQLiteDataReader reader = command.ExecuteReader();

                    country.Timezones = new List<string>();

                    while (reader.Read())
                    {
                        country.Timezones.Add(reader["timeZone"].ToString());
                    }
                });
            }
            catch (Exception e)
            {
                dialogService.ShowMessage("Error", e.Message);

            }
        }

        /// <summary>
        /// Receives a country's alpha3code and its respective list of timeZones string, and inserts it into the timeZones sqlite table using an SQLiteConnection
        /// </summary>
        /// <param name="timeZones"></param>
        /// <param name="alpha3code"></param>
        /// <param name="connection"></param>
        /// <returns></returns>
        public async Task SaveTimeZonesAsync(List<string> timeZones, string alpha3code, SQLiteConnection connection)
        {
            try
            {
                foreach (string timeZone in timeZones)
                {
                    command.Parameters.AddWithValue("@alpha3code", alpha3code);
                    command.Parameters.AddWithValue("@timeZone", timeZone);

                    command.CommandText = "insert into timeZones values(@alpha3code, @timeZone)";

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