namespace WPF_Project_Countries.Services
{
    #region Usings

    using System;
    using System.Collections.Generic;
    using System.Data.SQLite;
    using System.Threading.Tasks;
    using Library.Models;

    #endregion

    class TopLevelDomainService
    {
        #region Variables

        private SQLiteCommand command;

        private readonly DialogService dialogService = new DialogService();

        #endregion

        #region Constructor

        /// <summary>
        /// Parametrized constructor for the top level domains services, that receives an SQLiteConnection and creates an SQLite table for the TopLevelDomain string list
        /// </summary>
        /// <param name="connection"></param>
        public TopLevelDomainService(SQLiteConnection connection)
        {
            try
            {
                string sqlcommand = "create table if not exists topLevelDomains(alpha3code char(3), topLevelDomain varchar(10), foreign key (alpha3code) references countries(alpha3code));";

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
        /// Gets the topLevelDomains strings from the sqlite table topLevelsDomains and adds them to a country's topLevelDomains list
        /// </summary>
        /// <param name="country"></param>
        /// <param name="connection"></param>
        public async Task GetTopLevelDomains(Country country, SQLiteConnection connection)
        {
            try
            {
                await Task.Run(() =>
                {
                    string sql = $"select alpha3code, topLevelDomain from topLevelDomains where alpha3code = '{country.Alpha3Code}'";

                    command = new SQLiteCommand(sql, connection);

                    SQLiteDataReader reader = command.ExecuteReader();

                    country.TopLevelDomain = new List<string>();

                    while (reader.Read())
                    {
                        country.TopLevelDomain.Add(reader["topLevelDomain"].ToString());
                    }
                });
            }
            catch (Exception e)
            {
                dialogService.ShowMessage("Error", e.Message);

            }
        }

        /// <summary>
        /// Receives a country's alpha3code and its respective list of topLevelDomains string, and inserts it into the topLevelDomains sqlite table using an SQLiteConnection
        /// </summary>
        /// <param name="topLevelDomains"></param>
        /// <param name="alpha3code"></param>
        /// <param name="connection"></param>
        /// <returns></returns>
        public async Task SaveTopLevelDomainAsync(List<string> topLevelDomains, string alpha3code, SQLiteConnection connection)
        {
            try
            {
                foreach (string topLevelDomain in topLevelDomains)
                {
                    command.Parameters.AddWithValue("@alpha3code", alpha3code);
                    command.Parameters.AddWithValue("@topLevelDomain", topLevelDomain);

                    command.CommandText = "insert into topLevelDomains values(@alpha3code, @topLevelDomain)";

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