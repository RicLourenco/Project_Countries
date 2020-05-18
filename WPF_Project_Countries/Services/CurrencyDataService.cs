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

        public async Task SaveCurrencyAsync(Country country)
        {
            await Task.Run(() =>
            {
                foreach (Currency currency in country.Currencies)
                {
                    command.Parameters.AddWithValue("@alpha3code", country.Alpha3Code);
                    command.Parameters.AddWithValue("@code", currency.Code);
                    command.Parameters.AddWithValue("@name", currency.Name);
                    command.Parameters.AddWithValue("@symbol", currency.Symbol);

                    command.CommandText = "insert into currencies values(@alpha3code, @code, @name, @symbol)";

                    command.Connection = connection;

                    command.ExecuteNonQuery();
                }
            });
        }
    }
}
