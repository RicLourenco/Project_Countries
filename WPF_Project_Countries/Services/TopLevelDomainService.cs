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

        public async Task SaveTopLevelDomainAsync(List<string> topLevelDomains, string alpha3code)
        {
            await Task.Run(() =>
            {
                foreach (string topLevelDomain in topLevelDomains)
                {
                    command.Parameters.AddWithValue("@alpha3code", alpha3code);
                    command.Parameters.AddWithValue("@topLevelDomain", topLevelDomain);

                    command.CommandText = "insert into topLevelDomains values(@alpha3code, @topLevelDomain)";

                    command.Connection = connection;

                    command.ExecuteNonQuery();
                }
            });

            //await Task.Run(() => {
            //    foreach (string topLevelDomain in topLevelDomains)
            //    {
            //        string sql = $"insert into topLevelDomains values('{alpha3code}', '{topLevelDomain}')";

            //        command = new SQLiteCommand(sql, connection);

            //        command.ExecuteNonQuery();
            //    }
            //});
        }

        public void GetTopLevelDomains(Country country)
        {
            try
            {
                string sql = $"select alpha3code, topLevelDomain from topLevelDomains where alpha3code = '{country.Alpha3Code}'";

                command = new SQLiteCommand(sql, connection);

                SQLiteDataReader reader = command.ExecuteReader();

                country.TopLevelDomain = new List<string>();

                while (reader.Read())
                {
                    country.TopLevelDomain.Add(reader["topLevelDomain"].ToString());
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
