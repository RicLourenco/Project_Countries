namespace WPF_Project_Countries.Services
{
    using System;
    using System.Collections.Generic;
    using System.Data.SQLite;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Library.Models;

    class CurrencyDataService
    {
        private SQLiteConnection connection;

        private SQLiteCommand command;

        private DialogService dialogService = new DialogService();

        public CurrencyDataService()
        {
            var path = @"Data\Countries.sqlite";

            try
            {
                connection = new SQLiteConnection("Data Source=" + path);
                connection.Open();

                string sqlcommand = "create table if not exists currencies(alpha3code char(3), code char(3), name varchar(50), symbol varchar(10), foreign key(alpha3code) references country(alpha3code))";

                command = new SQLiteCommand(sqlcommand, connection);

                command.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                dialogService.ShowMessage("Erro", e.Message);
            }
        }

        public async Task SaveCurrencyAsync(List<Currency> currencies, string alpha3code)
        {
            await Task.Run(() =>
            {
                foreach (Currency currency in currencies)
                {
                    command.Parameters.AddWithValue("@alpha3code", alpha3code);
                    command.Parameters.AddWithValue("@code", currency.Code);
                    command.Parameters.AddWithValue("@name", currency.Name);
                    command.Parameters.AddWithValue("@symbol", currency.Symbol);

                    command.CommandText = "insert into currencies values(@alpha3code, @code, @name, @symbol)";

                    command.Connection = connection;

                    command.ExecuteNonQuery();
                }
            });

            //await Task.Run(() => {
            //    foreach (Currency currency in currencies)
            //    {
            //        string sql = $"insert into currencies values('{alpha3code}', '{currency.Code}', \"{currency.Name}\", \"{currency.Symbol}\")";

            //        command = new SQLiteCommand(sql, connection);

            //        command.ExecuteNonQuery();
            //    }
            //});
        }

        public void GetCurrencies(Country country)
        {
            try
            {
                string sql = $"select alpha3code, code, name, symbol from currencies where alpha3code = '{country.Alpha3Code}'";

                command = new SQLiteCommand(sql, connection);

                SQLiteDataReader reader = command.ExecuteReader();

                country.Currencies = new List<Currency>();

                while (reader.Read())
                {
                    country.Currencies.Add(new Currency { 
                    Code = reader["code"].ToString(),
                    Name = reader["name"].ToString(),
                    Symbol = reader["symbol"].ToString()
                    });
                }

                //connection.Close();
            }
            catch (Exception e)
            {
                dialogService.ShowMessage("Erro", e.Message);
            }
        }
    }
}
