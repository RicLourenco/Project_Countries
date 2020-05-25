namespace WPF_Project_Countries.Services
{
    using Library.Models;
    using System;
    using System.Collections.Generic;
    using System.Data.SQLite;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    class LatlngsService
    {
        private SQLiteConnection connection;

        private SQLiteCommand command;

        private DialogService dialogService = new DialogService();

        public LatlngsService()
        {
            var path = @"Data\Countries.sqlite";

            try
            {
                connection = new SQLiteConnection("Data Source=" + path);
                connection.Open();

                string sqlcommand = "create table if not exists latlngs(alpha3code char(3), latlng real, foreign key (alpha3code) references countries(alpha3code));";

                command = new SQLiteCommand(sqlcommand, connection);

                command.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                dialogService.ShowMessage("Erro", e.Message);
            }
        }

        public async Task SaveLatlngsAsync(List<double> latlngs, string alpha3code)
        {
            await Task.Run(() =>
            {
                foreach (double latlng in latlngs)
                {
                    command.Parameters.AddWithValue("@alpha3code", alpha3code);
                    command.Parameters.AddWithValue("@latlng", latlng);

                    command.CommandText = "insert into latlngs values(@alpha3code, @latlng)";

                    command.Connection = connection;

                    command.ExecuteNonQuery();
                }
            });

            //await Task.Run(() => {
            //    foreach (double latlng in latlngs)
            //    {
            //        string sql = $"insert into latlngs values('{alpha3code}', '{latlng}')";

            //        command = new SQLiteCommand(sql, connection);

            //        command.ExecuteNonQuery();
            //    }
            //});
        }

        public void GetLatlngs(Country country)
        {
            try
            {
                string sql = $"select alpha3code, latlng from latlngs where alpha3code = '{country.Alpha3Code}'";

                command = new SQLiteCommand(sql, connection);

                SQLiteDataReader reader = command.ExecuteReader();

                country.Latlng = new List<double>();

                while (reader.Read())
                {
                    country.Latlng.Add(Convert.ToDouble(reader["latlng"]));
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
