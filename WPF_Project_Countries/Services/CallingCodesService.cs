namespace WPF_Project_Countries.Services
{
    #region Usings

    using System;
    using System.Collections.Generic;
    using System.Data.SQLite;
    using System.Threading.Tasks;
    using Library.Models;

    #endregion

    class CallingCodesService
    {
        #region Variables

        private SQLiteCommand command;

        private readonly DialogService dialogService = new DialogService();

        #endregion

        #region Constructor

        /// <summary>
        /// Parametrized constructor for the calling codes services, that receives an SQLiteConnection and creates an SQLite table for the CallingCodes string list
        /// </summary>
        /// <param name="connection"></param>
        public CallingCodesService(SQLiteConnection connection)
        {
            try
            {
                string sqlcommand = "create table if not exists callingCodes(alpha3code char(3), callingCode varchar(10), foreign key (alpha3code) references countries(alpha3code));";

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
        /// Gets the callingCodes strings from the sqlite table callingCodes and adds them to a country's callingCodes list
        /// </summary>
        /// <param name="country"></param>
        /// <param name="connection"></param>
        /// <returns></returns>
        public async Task GetCallingCodes(Country country, SQLiteConnection connection)
        {
            try
            {
                await Task.Run(() =>
                {
                    string sql = $"select alpha3code, callingCode from callingCodes where alpha3code = '{country.Alpha3Code}'";

                    command = new SQLiteCommand(sql, connection);

                    SQLiteDataReader reader = command.ExecuteReader();

                    country.CallingCodes = new List<string>();

                    while (reader.Read())
                    {
                        country.CallingCodes.Add(reader["callingCode"].ToString());
                    };
                });
            }
            catch (Exception e)
            {
                dialogService.ShowMessage("Error", e.Message);
            }
        }

        /// <summary>
        /// Receives a country's alpha3code and its respective list of callingCodes string, and inserts it into the callingCodes sqlite table using an SQLiteConnection
        /// </summary>
        /// <param name="callingCodes"></param>
        /// <param name="alpha3code"></param>
        /// <param name="connection"></param>
        /// <returns></returns>
        public async Task SaveCallingCodesAsync(List<string> callingCodes, string alpha3code, SQLiteConnection connection)
        {
            try
            {
                foreach (string callingCode in callingCodes)
                {
                    command.Parameters.AddWithValue("@alpha3code", alpha3code);
                    command.Parameters.AddWithValue("@callingCode", callingCode);

                    command.CommandText = "insert into callingCodes values(@alpha3code, @callingCode)";

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