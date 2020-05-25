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

        public async Task SaveLanguageAsync(List<Language> languages, string alpha3code)
        {
            await Task.Run(() =>
            {
                foreach (Language language in languages)
                {
                    command.Parameters.AddWithValue("@alpha3code", alpha3code);
                    command.Parameters.AddWithValue("@iso639_1", language.Iso639_1);
                    command.Parameters.AddWithValue("@iso639_2", language.Iso639_2);
                    command.Parameters.AddWithValue("@name", language.Name);
                    command.Parameters.AddWithValue("@nativeName", language.NativeName);

                    command.CommandText = "insert into languages values(@alpha3code, @iso639_1, @iso639_2, @name, @nativeName )";

                    command.Connection = connection;

                    command.ExecuteNonQuery();
                }
            });

            //await Task.Run(() => {
            //    foreach (Language language in languages)
            //    {
            //        string sql =  $"insert into languages values('{alpha3code}', '{language.Iso639_1}', '{language.Iso639_2}', \"{language.Name}\", \"{language.NativeName}\")";

            //        command = new SQLiteCommand(sql, connection);

            //        command.ExecuteNonQuery();
            //    }
            //});
        }

        public void GetLanguages(Country country)
        {
            try
            {
                string sql = $"select alpha3code, iso639_1, iso639_2, name, nativeName from languages where alpha3code = '{country.Alpha3Code}'";

                command = new SQLiteCommand(sql, connection);

                SQLiteDataReader reader = command.ExecuteReader();

                country.Languages = new List<Language>();

                while (reader.Read())
                {
                    country.Languages.Add(new Language
                    {
                        Iso639_1 = reader["iso639_1"].ToString(),
                        Iso639_2 = reader["iso639_2"].ToString(),
                        Name = reader["name"].ToString(),
                        NativeName = reader["nativeName"].ToString()
                    });
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
