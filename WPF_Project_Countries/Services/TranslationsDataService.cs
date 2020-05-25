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

        public async Task SaveTranslationsAsync(Translations translations, string alpha3code)
        {
            await Task.Run(() =>
            {
                command.Parameters.AddWithValue("@alpha3code", alpha3code);
                command.Parameters.AddWithValue("@de", translations.De);
                command.Parameters.AddWithValue("@es", translations.Es);
                command.Parameters.AddWithValue("@fr", translations.Fr);
                command.Parameters.AddWithValue("@ja", translations.Ja);
                command.Parameters.AddWithValue("@it", translations.It);
                command.Parameters.AddWithValue("@br", translations.Br);
                command.Parameters.AddWithValue("@pt", translations.Pt);
                command.Parameters.AddWithValue("@nl", translations.Nl);
                command.Parameters.AddWithValue("@hr", translations.Hr);
                command.Parameters.AddWithValue("@fa", translations.Fa);

                command.CommandText = "insert into translations values(@alpha3code, @de, @es, @fr, @ja, @it, @br, @pt, @nl, @hr, @fa)";

                command.Connection = connection;

                command.ExecuteNonQuery();
            });

            //await Task.Run(() => {
            //    string sql = $"insert into translations values('{alpha3code}', \"{translations.De}\", \"{translations.Es}\", \"{translations.Fr}\", \"{translations.Ja}\", \"{translations.It}\", \"{translations.Br}\", \"{translations.Pt}\", \"{translations.Nl}\", \"{translations.Hr}\", \"{translations.Fa}\")";

            //    command = new SQLiteCommand(sql, connection);

            //    command.ExecuteNonQuery();
            //});
        }

        public void GetTranslations(Country country)
        {
            try
            {
                string sql = $"select alpha3code, de, es, fr, ja, it, br, pt, nl, hr, fa from translations where alpha3code = '{country.Alpha3Code}'";

                command = new SQLiteCommand(sql, connection);

                SQLiteDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    country.Translations = (new Translations
                    {
                        De = reader["de"].ToString(),
                        Es = reader["es"].ToString(),
                        Fr = reader["fr"].ToString(),
                        Ja = reader["ja"].ToString(),
                        It = reader["it"].ToString(),
                        Br = reader["br"].ToString(),
                        Pt = reader["pt"].ToString(),
                        Nl = reader["nl"].ToString(),
                        Hr = reader["hr"].ToString(),
                        Fa = reader["fa"].ToString()
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
