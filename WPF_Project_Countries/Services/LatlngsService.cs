namespace WPF_Project_Countries.Services
{
    #region Usings

    using System;
    using System.Collections.Generic;
    using System.Data.SQLite;
    using System.Threading.Tasks;
    using Library.Models;

    #endregion

    class LatlngsService
    {
        #region Variables

        private SQLiteCommand command;

        private readonly DialogService dialogService = new DialogService();

        #endregion

        #region Constructor

        /// <summary>
        /// Parametrized constructor for the latitude and logitudes services, that receives an SQLiteConnection and creates an SQLite table for the Latlng double list
        /// </summary>
        /// <param name="connection"></param>
        public LatlngsService( SQLiteConnection connection)
        {
            try
            {
                string sqlcommand = "create table if not exists latlngs(alpha3code char(3), latlng real, foreign key (alpha3code) references countries(alpha3code));";

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
        /// Gets the latlngs doubles from the sqlite table latlngs and adds them to a country's latlngs list
        /// </summary>
        /// <param name="country"></param>
        /// <param name="connection"></param>
        public async Task GetLatlngs(Country country, SQLiteConnection connection)
        {
            try
            {
                await Task.Run(() =>
                {
                    string sql = $"select alpha3code, latlng from latlngs where alpha3code = '{country.Alpha3Code}'";

                    command = new SQLiteCommand(sql, connection);

                    SQLiteDataReader reader = command.ExecuteReader();

                    country.Latlng = new List<double>();

                    while (reader.Read())
                    {
                        country.Latlng.Add(Convert.ToDouble(reader["latlng"]));
                    }
                });
            }
            catch (Exception e)
            {
                dialogService.ShowMessage("Error", e.Message);
            }
        }

        /// <summary>
        /// Receives a country's alpha3code and its respective list of Latlngs double, and inserts it into the latlngs sqlite table using an SQLiteConnection
        /// </summary>
        /// <param name="latlngs"></param>
        /// <param name="alpha3code"></param>
        /// <param name="connection"></param>
        /// <returns></returns>
        public async Task SaveLatlngsAsync(List<double> latlngs, string alpha3code, SQLiteConnection connection)
        {
            try
            {
                foreach (double latlng in latlngs)
                {
                    command.Parameters.AddWithValue("@alpha3code", alpha3code);
                    command.Parameters.AddWithValue("@latlng", latlng);

                    command.CommandText = "insert into latlngs values(@alpha3code, @latlng)";

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