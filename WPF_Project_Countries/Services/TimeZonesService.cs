namespace WPF_Project_Countries.Services
{
    using Library.Models;
    using System;
    using System.Collections.Generic;
    using System.Data.SQLite;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    class TimeZonesService
    {
        private SQLiteConnection connection;

        private SQLiteCommand command;

        private DialogService dialogService = new DialogService();

        public TimeZonesService()
        {
            var path = @"Data\Countries.sqlite";

            try
            {
                connection = new SQLiteConnection("Data Source=" + path);
                connection.Open();

                string sqlcommand = "create table if not exists timeZones(alpha3code char(3), timeZone varchar(15), foreign key (alpha3code) references countries(alpha3code));";

                command = new SQLiteCommand(sqlcommand, connection);

                command.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                dialogService.ShowMessage("Erro", e.Message);
            }
        }

        public async Task SaveTimeZonesAsync(Country country)
        {
            await Task.Run(() => {
                foreach (string timeZone in country.Timezones)
                {
                    command.Parameters.AddWithValue("@alpha3code", country.Alpha3Code);
                    command.Parameters.AddWithValue("@timeZone", timeZone);

                    command.CommandText = "insert into timeZones values(@alpha3code, @timeZone)";

                    command.Connection = connection;

                    command.ExecuteNonQuery();
                }
            });
        }

        public void GetTimeZones(List<Country> countries)
        {
            try
            {
                string sql = "select timeZone from timeZones";

                command = new SQLiteCommand(sql, connection);

                SQLiteDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    foreach (var country in countries)
                    {
                        while (reader["alpha3code"].ToString() == country.Alpha3Code)
                        {
                            country.Timezones.Add(reader["timeZone"].ToString());
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
