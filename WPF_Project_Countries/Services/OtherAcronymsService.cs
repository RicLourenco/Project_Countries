namespace WPF_Project_Countries.Services
{
    using Library.Models;
    using System;
    using System.Collections.Generic;
    using System.Data.SQLite;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    class OtherAcronymsService
    {
        private SQLiteConnection connection;

        private SQLiteCommand command;

        private DialogService dialogService;

        public OtherAcronymsService()
        {
            var path = @"Data\Countries.sqlite";

            try
            {
                connection = new SQLiteConnection("Data Source=" + path);
                connection.Open();

                string sqlcommand = "create table if not exists otherAcronyms(id_regionalBloc integer, otherAcronym varchar(50), foreign key (id_regionalBloc) references regionalBloc(id));";

                command = new SQLiteCommand(sqlcommand, connection);

                command.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                dialogService.ShowMessage("Erro", e.Message);
            }
        }

        public async Task SaveOtherAcronymsAsync(RegionalBloc regionalBloc)
        {
            await Task.Run(() =>
            {
                foreach (string otherAcronyms in regionalBloc.OtherAcronyms)
                {
                    //command.Parameters.AddWithValue("@id_regionalBloc", );
                    command.Parameters.AddWithValue("@otherAcronym", regionalBloc.Acronym);

                    command.CommandText = "insert into regionalBlocs values(@id_regionalBloc, @otherAcronym)";

                    command.Connection = connection;

                    command.ExecuteNonQuery();
                }
            });
        }

        private void GetOtherAcronyms(List<Country> countries)
        {
            /*
            try
            {
                string sql = "select otherAcronym from otherAcronyms";

                command = new SQLiteCommand(sql, connection);

                SQLiteDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    foreach (var country in countries)
                    {
                        foreach(var regionalBloc in country.RegionalBlocs)
                        {
                            while (reader["id_regionalBloc"].ToString() == country.RegionalBlocs)
                            {
                                country.TopLevelDomain.Add(reader["otherAcronym"].ToString());
                            }
                        }
                    }

                }

                connection.Close();
            }
            catch (Exception e)
            {
                dialogService.ShowMessage("Erro", e.Message);
            }
            */
        }
    }
}
