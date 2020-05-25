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

                string sqlcommand = "create table if not exists otherAcronyms(id_regionalBloc int, otherAcronym varchar(50), foreign key (id_regionalBloc) references regionalBloc(id));";

                command = new SQLiteCommand(sqlcommand, connection);

                command.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                dialogService.ShowMessage("Erro", e.Message);
            }
        }

        public async Task SaveOtherAcronymsAsync(List<string> otherAcronyms)
        {
            await Task.Run(() =>
            {
                foreach (string otherAcronym in otherAcronyms)
                {
                    command.Parameters.AddWithValue("@otherAcronym", otherAcronym);

                    command.CommandText = "insert into otherAcronyms values((select id from regionalBlocs order by id desc limit 1), @otherAcronym)";

                    command.Connection = connection;

                    command.ExecuteNonQuery();
                }
            });

            //await Task.Run(() => {
            //    foreach (string otherAcronym in otherAcronyms)
            //    {
            //        string sql = $"insert into otherAcronyms values((select id from regionalBlocs order by id desc limit 1), '{otherAcronym}')";

            //        command = new SQLiteCommand(sql, connection);

            //        command.ExecuteNonQuery();
            //    }
            //});
        }

        public List<string> GetOtherAcronyms(string id)
        {
            try
            {
                string sql = $"select otherAcronym from otherAcronyms where id_regionalBloc = '{id}'";

                command = new SQLiteCommand(sql, connection);

                SQLiteDataReader reader = command.ExecuteReader();

                List<string> otherAcronyms = new List<string>();

                while (reader.Read())
                {
                    otherAcronyms.Add(reader["otherAcronym"].ToString());
                }

                return otherAcronyms;

                //connection.Close();
            }
            catch (Exception e)
            {
                dialogService.ShowMessage("Erro", e.Message);
                return null;
            }
        }
    }
}
