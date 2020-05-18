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

        public async Task SaveLatlngsAsync(Country country)
        {
            await Task.Run(() => {
                foreach (double latlng in country.Latlng)
                {
                    command.Parameters.AddWithValue("@alpha3code", country.Alpha3Code);
                    command.Parameters.AddWithValue("@latlng", latlng);

                    command.CommandText = "insert into latlngs values(@alpha3code, @latlng)";

                    command.Connection = connection;

                    command.ExecuteNonQuery();
                }
            });
        }

        public void GetLatlngs(List<Country> countries)
        {
            try
            {
                string sql = "select alpha3code, latlng from latlngs";

                command = new SQLiteCommand(sql, connection);

                SQLiteDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    foreach (var country in countries)
                    {
                        while (reader["alpha3code"].ToString() == country.Alpha3Code)
                        {
                            country.Latlng.Add(Convert.ToDouble(reader["latlng"]));
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
