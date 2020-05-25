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

        public async Task SaveTimeZonesAsync(List<string> timeZones, string alpha3code)
        {
            await Task.Run(() =>
            {
                foreach (string timeZone in timeZones)
                {
                    command.Parameters.AddWithValue("@alpha3code", alpha3code);
                    command.Parameters.AddWithValue("@timeZone", timeZone);

                    command.CommandText = "insert into timeZones values(@alpha3code, @timeZone)";

                    command.Connection = connection;

                    command.ExecuteNonQuery();
                }
            });

            //await Task.Run(() => {
            //    foreach (string timeZone in timeZones)
            //    {
            //        string sql = $"insert into timeZones values('{alpha3code}', '{timeZone}')";

            //        command = new SQLiteCommand(sql, connection);

            //        command.ExecuteNonQuery();
            //    }
            //});
        }

        public void GetTimeZones(Country country)
        {
            try
            {
                string sql = $"select alpha3code, timeZone from timeZones where alpha3code = '{country.Alpha3Code}'";

                command = new SQLiteCommand(sql, connection);

                SQLiteDataReader reader = command.ExecuteReader();

                country.Timezones = new List<string>();

                while (reader.Read())
                {
                    country.Timezones.Add(reader["timeZone"].ToString());
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