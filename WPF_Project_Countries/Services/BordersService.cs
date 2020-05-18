namespace WPF_Project_Countries.Services
{
    using Library.Models;
    using System;
    using System.Collections.Generic;
    using System.Data.SQLite;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    class BordersService
    {
        private SQLiteConnection connection;

        private SQLiteCommand command;

        private DialogService dialogService = new DialogService();

        public BordersService()
        {
            var path = @"Data\Countries.sqlite";

            try
            {
                connection = new SQLiteConnection("Data Source=" + path);
                connection.Open();

                string sqlcommand = "create table if not exists borders(alpha3code char(3), border char(3), foreign key (alpha3code) references countries(alpha3code))";

                command = new SQLiteCommand(sqlcommand, connection);

                command.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                dialogService.ShowMessage("Erro", e.Message);
            }
        }

        public async Task SaveBordersAsync(Country country)
        {
            await Task.Run(() =>
            {
                foreach (string border in country.Borders)
                {
                    command.Parameters.AddWithValue("@alpha3code", country.Alpha3Code);
                    command.Parameters.AddWithValue("@border", border);

                    command.CommandText = "insert into borders values(@alpha3code, @border)";

                    command.Connection = connection;

                    command.ExecuteNonQuery();
                }
            });
        }

        private void GetBorders(List<Country> countries)
        {
            try
            {
                string sql = "select border from borders";

                command = new SQLiteCommand(sql, connection);

                SQLiteDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    foreach (var country in countries)
                    {
                        while (reader["alpha3code"].ToString() == country.Alpha3Code)
                        {
                            country.Borders.Add(reader["border"].ToString());
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
