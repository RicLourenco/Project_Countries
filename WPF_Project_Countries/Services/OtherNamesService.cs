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
    }
}
