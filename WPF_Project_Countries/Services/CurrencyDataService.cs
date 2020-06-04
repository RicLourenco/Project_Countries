namespace WPF_Project_Countries.Services
{
    #region Usings

    using System;
    using System.Collections.Generic;
    using System.Data.SQLite;
    using System.Threading.Tasks;
    using Library.Models;

    #endregion

    class CurrencyDataService
    {
        #region Variables

        private SQLiteCommand command;

        private readonly DialogService dialogService = new DialogService();

        #endregion

        #region Constructor

        /// <summary>
        /// Parametrized constructor for the currencies services, that receives an SQLiteConnection and creates an SQLite table for the Currency object
        /// </summary>
        /// <param name="connection"></param>
        public CurrencyDataService(SQLiteConnection connection)
        {
            try
            {
                string sqlcommand = "create table if not exists currencies(alpha3code char(3), code char(3), name varchar(50), symbol varchar(10), foreign key(alpha3code) references country(alpha3code))";

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
        /// Gets the currencies objects from the sqlite table currencies and adds them to a country's currencies list
        /// </summary>
        /// <param name="country"></param>
        /// <param name="connection"></param>
        /// <returns></returns>
        public async Task GetCurrencies(Country country, SQLiteConnection connection)
        {
            try
            {
                await Task.Run(() =>
                {
                    string sql = $"select alpha3code, code, name, symbol from currencies where alpha3code = '{country.Alpha3Code}'";

                    command = new SQLiteCommand(sql, connection);

                    SQLiteDataReader reader = command.ExecuteReader();

                    country.Currencies = new List<Currency>();

                    while (reader.Read())
                    {
                        country.Currencies.Add(new Currency
                        {
                            Code = reader["code"].ToString(),
                            Name = reader["name"].ToString(),
                            Symbol = reader["symbol"].ToString()
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
        /// Receives a country's alpha3code and its respective list of currencies object, and inserts it into the currencies sqlite table using an SQLiteConnection
        /// </summary>
        /// <param name="currencies"></param>
        /// <param name="alpha3code"></param>
        /// <param name="connection"></param>
        /// <returns></returns>
        public async Task SaveCurrencyAsync(List<Currency> currencies, string alpha3code, SQLiteConnection connection)
        {
            try
            {
                foreach (Currency currency in currencies)
                {
                    command.Parameters.AddWithValue("@alpha3code", alpha3code);
                    command.Parameters.AddWithValue("@code", currency.Code);
                    command.Parameters.AddWithValue("@name", currency.Name);
                    command.Parameters.AddWithValue("@symbol", currency.Symbol);

                    command.CommandText = "insert into currencies values(@alpha3code, @code, @name, @symbol)";

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