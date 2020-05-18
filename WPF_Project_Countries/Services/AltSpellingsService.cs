namespace WPF_Project_Countries.Services
{
    using Library.Models;
    using System;
    using System.Collections.Generic;
    using System.Data.SQLite;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    class AltSpellingsService
    {
        private SQLiteConnection connection;

        private SQLiteCommand command;

        private DialogService dialogService = new DialogService();

        public AltSpellingsService()
        {
            var path = @"Data\Countries.sqlite";

            try
            {
                connection = new SQLiteConnection("Data Source=" + path);
                connection.Open();

                string sqlcommand = "create table if not exists altSpellings(alpha3code char(3), altSpelling varchar(50), foreign key (alpha3code) references countries(alpha3code));";

                command = new SQLiteCommand(sqlcommand, connection);

                command.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                dialogService.ShowMessage("Erro", e.Message);
            }
        }

        public async Task SaveAltSpellings(Country country)
        {
            await Task.Run(() => {
                foreach (string altSpelling in country.AltSpellings)
                {
                    command.Parameters.AddWithValue("@alpha3code", country.Alpha3Code);
                    command.Parameters.AddWithValue("@altSpelling", altSpelling);

                    command.CommandText = "insert into altSpellings values(@alpha3code, @altSpelling)";

                    command.Connection = connection;

                    command.ExecuteNonQuery();
                }
            });
            
        }

        public void GetAltSpellings(List<Country> countries)
        {
            try
            {
                string sql = "select alpha3code, altSpelling from altSpellings";

                command = new SQLiteCommand(sql, connection);

                SQLiteDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    foreach (var country in countries)
                    {
                        while (reader["alpha3code"].ToString() == country.Alpha3Code)
                        {
                            country.AltSpellings.Add(reader["altSpelling"].ToString());
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
