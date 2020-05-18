namespace WPF_Project_Countries.Services
{
    using Library.Models;
    using System;
    using System.Collections.Generic;
    using System.Data.SQLite;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    class TopLevelDomainService
    {
        private SQLiteConnection connection;

        private SQLiteCommand command;

        private DialogService dialogService = new DialogService();

        public TopLevelDomainService()
        {
            var path = @"Data\Countries.sqlite";

            try
            {
                connection = new SQLiteConnection("Data Source=" + path);
                connection.Open();

                string sqlcommand = "create table if not exists topLevelDomains(alpha3code char(3), topLevelDomain varchar(10), foreign key (alpha3code) references countries(alpha3code));";

                command = new SQLiteCommand(sqlcommand, connection);

                command.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                dialogService.ShowMessage("Erro", e.Message);
            }
        }

        public async Task SaveTopLevelDomainAsync(Country country)
        {
            await Task.Run(() => {
                foreach (string topLevelDomain in country.TopLevelDomain)
                {
                    command.Parameters.AddWithValue("@alpha3code", country.Alpha3Code);
                    command.Parameters.AddWithValue("@topLevelDomain", topLevelDomain);

                    command.CommandText = "insert into topLevelDomains values(@alpha3code, @topLevelDomain)";

                    command.Connection = connection;

                    command.ExecuteNonQuery();
                }
            });
        }

        private void GetTopLevelDomains(List<Country> countries)
        {
            try
            {
                string sql = "select topLevelDomain from topLevelDomains";

                command = new SQLiteCommand(sql, connection);

                SQLiteDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    foreach (var country in countries)
                    {
                        while (reader["alpha3code"].ToString() == country.Alpha3Code)
                        {
                            country.TopLevelDomain.Add(reader["topLevelDomain"].ToString());
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
