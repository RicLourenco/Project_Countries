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

        public async Task SaveAltSpellings(List<string> altSpellings, string alpha3code)
        {
            await Task.Run(() =>
            {
                foreach (string altSpelling in altSpellings)
                {
                    command.Parameters.AddWithValue("@alpha3code", alpha3code);
                    command.Parameters.AddWithValue("@altSpelling", altSpelling);

                    command.CommandText = "insert into altSpellings values(@alpha3code, @altSpelling)";

                    command.Connection = connection;

                    command.ExecuteNonQuery();
                }
            });

            //await Task.Run(() => {
            //    foreach (string altSpelling in altSpellings)
            //    {
            //        string sql = $"insert into altSpellings values('{alpha3code}', \"{altSpelling}\")";

            //        command = new SQLiteCommand(sql, connection);

            //        command.ExecuteNonQuery();
            //    }
            //});

            //connection.Close();

        }

        public void GetAltSpellings(Country country)
        {
            try
            {
                string sql = $"select alpha3code, altSpelling from altSpellings where alpha3code = '{country.Alpha3Code}'";

                command = new SQLiteCommand(sql, connection);

                SQLiteDataReader reader = command.ExecuteReader();

                country.AltSpellings = new List<string>();

                while (reader.Read())
                {
                    country.AltSpellings.Add(reader["altSpelling"].ToString());
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
