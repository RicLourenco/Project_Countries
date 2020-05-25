using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF_Project_Countries.Services
{
    class OtherNamesService
    {
        private SQLiteConnection connection;

        private SQLiteCommand command;

        private DialogService dialogService;

        public OtherNamesService()
        {
            var path = @"Data\Countries.sqlite";

            try
            {
                connection = new SQLiteConnection("Data Source=" + path);
                connection.Open();

                string sqlcommand = "create table if not exists otherNames(id_regionalBloc integer, otherName varchar(50), foreign key (id_regionalBloc) references regionalBloc(id))";

                command = new SQLiteCommand(sqlcommand, connection);

                command.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                dialogService.ShowMessage("Erro", e.Message);
            }
        }

        public async Task SaveOtherNamesAsync(List<string> otherNames)
        {
            await Task.Run(() =>
            {
                foreach (string otherName in otherNames)
                {
                    command.Parameters.AddWithValue("@otherName", otherName);

                    command.CommandText = "insert into otherNames values((select id from regionalBlocs order by id desc limit 1), @otherName)";

                    command.Connection = connection;

                    command.ExecuteNonQuery();
                }
            });

            //await Task.Run(() => {
            //    foreach (string otherName in otherNames)
            //    {
            //        string sql = $"insert into otherNames values((select id from regionalBlocs order by id desc limit 1), \"{otherName}\")";

            //        command = new SQLiteCommand(sql, connection);

            //        command.ExecuteNonQuery();
            //    }
            //});
        }

        public List<string> GetOtherNames(string id)
        {
            try
            {
                string sql = $"select otherName from otherNames where id_regionalBloc = '{id}'";

                command = new SQLiteCommand(sql, connection);

                SQLiteDataReader reader = command.ExecuteReader();

                List<string> otherNames = new List<string>();

                while (reader.Read())
                {
                    otherNames.Add(reader["otherName"].ToString());
                }

                return otherNames;

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
