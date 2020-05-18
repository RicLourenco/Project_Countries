namespace WPF_Project_Countries.Services
{
    using Library.Models;
    using System;
    using System.Collections.Generic;
    using System.Data.SQLite;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    class CallingCodesService
    {
        private SQLiteConnection connection;

        private SQLiteCommand command;

        private DialogService dialogService = new DialogService();

        public CallingCodesService()
        {
            var path = @"Data\Countries.sqlite";

            try
            {
                connection = new SQLiteConnection("Data Source=" + path);
                connection.Open();

                string sqlcommand = "create table if not exists callingCodes(alpha3code char(3), topLevelDomain varchar(10), foreign key (alpha3code) references countries(alpha3code));";

                command = new SQLiteCommand(sqlcommand, connection);

                command.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                dialogService.ShowMessage("Erro", e.Message);
            }
        }

        public async Task SaveCallingCodesAsync(Country country)
        {
            await Task.Run(() => {
                foreach (string callingCode in country.CallingCodes)
                {
                    command.Parameters.AddWithValue("@alpha3code", country.Alpha3Code);
                    command.Parameters.AddWithValue("@callingCode", callingCode);

                    command.CommandText = "insert into callingCodes values(@alpha3code, @callingCode)";

                    command.Connection = connection;

                    command.ExecuteNonQuery();
                }
            });
        }

        public void GetCallingCodes(List<Country> countries)
        {
            try
            {
                string sql = "select * from callingCodes";

                command = new SQLiteCommand(sql, connection);

                SQLiteDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    foreach (var country in countries)
                    {
                        while (reader["alpha3code"].ToString() == country.Alpha3Code)
                        {
                            country.CallingCodes.Add(reader["callingCode"].ToString());
                        }
                    }

                }

                connection.Close();
            }
            catch (Exception e)
            {
                dialogService.ShowMessage("Erro", e.Message);
            }
        }
    }
}
