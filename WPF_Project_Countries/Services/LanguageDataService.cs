namespace WPF_Project_Countries.Services
{
    using Library.Models;
    using System;
    using System.Collections.Generic;
    using System.Data.SQLite;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    class LanguageDataService
    {
        private SQLiteConnection connection;

        private SQLiteCommand command;

        private DialogService dialogService = new DialogService();

        public LanguageDataService()
        {
            var path = @"Data\Countries.sqlite";

            try
            {
                connection = new SQLiteConnection("Data Source=" + path);
                connection.Open();

                string sqlcommand = "create table if not exists languages(alpha3code char(3), iso639_1 char(2), iso639_2 char(3), name varchar(50), nativeName varchar(50), foreign key(alpha3code) references country(alpha3code))";

                command = new SQLiteCommand(sqlcommand, connection);

                command.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                dialogService.ShowMessage("Erro", e.Message);
            }
        }

        public async Task SaveLanguageAsync(Country country)
        {
            await Task.Run(() =>
            {
                foreach (Language language in country.Languages)
                {
                    command.Parameters.AddWithValue("@alpha3code", country.Alpha3Code);
                    command.Parameters.AddWithValue("@iso639_1", language.Iso639_1);
                    command.Parameters.AddWithValue("@iso639_2", language.Iso639_2);
                    command.Parameters.AddWithValue("@name", language.Name);
                    command.Parameters.AddWithValue("@nativeName", language.NativeName);

                    command.CommandText = "insert into languages values(@alpha3code, @iso639_1, @iso639_2, @name, @nativeName )";

                    command.Connection = connection;

                    command.ExecuteNonQuery();
                }
            });
        }
    }
}
