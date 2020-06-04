namespace WPF_Project_Countries.Services
{
    #region Usings

    using System;
    using System.Collections.Generic;
    using System.Data.SQLite;
    using System.Threading.Tasks;
    using Library.Models;

    #endregion

    class BordersService
    {
        #region Variables

        private SQLiteCommand command;

        private readonly DialogService dialogService = new DialogService();

        #endregion

        #region Constructor

        /// <summary>
        /// Parametrized constructor for the borders services, that receives an SQLiteConnection and creates an SQLite table for the Border string list
        /// </summary>
        /// <param name="connection"></param>
        public BordersService(SQLiteConnection connection)
        {
            try
            {
                string sqlcommand = "create table if not exists borders(alpha3code char(3), border char(3), foreign key (alpha3code) references countries(alpha3code))";

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
        /// Receives a country's alpha3code and its respective list of borders string, and inserts it into the borders sqlite table using an SQLiteConnection
        /// </summary>
        /// <param name="borders"></param>
        /// <param name="alpha3code"></param>
        /// <param name="connection"></param>
        /// <returns></returns>
        public async Task SaveBordersAsync(List<string> borders, string alpha3code, SQLiteConnection connection)
        {
            try
            {
                foreach (string border in borders)
                {
                    command.Parameters.AddWithValue("@alpha3code", alpha3code);
                    command.Parameters.AddWithValue("@border", border);

                    command.CommandText = "insert into borders values(@alpha3code, @border)";

                    command.Connection = connection;

                    await command.ExecuteNonQueryAsync();
                }
            }
            catch (Exception e)
            {
                dialogService.ShowMessage("Error", e.Message);
            }
        }

        /// <summary>
        /// Gets the borders strings from the sqlite table borders and adds them to a country's borders list
        /// </summary>
        /// <param name="country"></param>
        /// <param name="connection"></param>
        /// <returns></returns>
        public async Task GetAltBorders(Country country, SQLiteConnection connection)
        {
            try
            {
                await Task.Run(() =>
                {
                    string sql = $"select alpha3code, border from borders where alpha3code = '{country.Alpha3Code}'";

                    command = new SQLiteCommand(sql, connection);

                    SQLiteDataReader reader = command.ExecuteReader();

                    country.Borders = new List<string>();

                    while (reader.Read())
                    {
                        country.Borders.Add(reader["border"].ToString());
                    }
                });
            }
            catch (Exception e)
            {
                dialogService.ShowMessage("Error", e.Message);
            }
        }

        #endregion
    }
}