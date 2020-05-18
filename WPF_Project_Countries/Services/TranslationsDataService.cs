namespace WPF_Project_Countries.Services
{
    using Library.Models;
    using System;
    using System.Collections.Generic;
    using System.Data.SQLite;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    class TranslationsDataService
    {
        private SQLiteConnection connection;

        private SQLiteCommand command;

        private DialogService dialogService = new DialogService();

        public TranslationsDataService()
        {
            var path = @"Data\Countries.sqlite";

            try
            {
                connection = new SQLiteConnection("Data Source=" + path);
                connection.Open();

                string sqlcommand = "create table if not exists translations(alpha3code char(3), de varchar(50), es varchar(50), fr varchar(50), ja varchar(50), it varchar(50), br varchar(50), pt varchar(50), nl varchar(50), hr varchar(50), fa varchar(50), foreign key(alpha3code) references country(alpha3code))";

                command = new SQLiteCommand(sqlcommand, connection);

                command.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                dialogService.ShowMessage("Erro", e.Message);
            }
        }

        public async Task SaveTranslationsAsync(Country country)
        {
            await Task.Run(() =>
            {
                command.Parameters.AddWithValue("@alpha3code", country.Alpha3Code);
                command.Parameters.AddWithValue("@de", country.Translations.De);
                command.Parameters.AddWithValue("@es", country.Translations.Es);
                command.Parameters.AddWithValue("@fr", country.Translations.Fr);
                command.Parameters.AddWithValue("@ja", country.Translations.Ja);
                command.Parameters.AddWithValue("@it", country.Translations.It);
                command.Parameters.AddWithValue("@br", country.Translations.Br);
                command.Parameters.AddWithValue("@pt", country.Translations.Pt);
                command.Parameters.AddWithValue("@nl", country.Translations.Nl);
                command.Parameters.AddWithValue("@hr", country.Translations.Hr);
                command.Parameters.AddWithValue("@fa", country.Translations.Fa);

                command.CommandText = "insert into translations values(@alpha3code, @de, @es, @fr, @ja, @it, @br, @pt, @nl, @hr, @fa)";

                command.Connection = connection;

                command.ExecuteNonQuery();
            });
        }
    }
}
