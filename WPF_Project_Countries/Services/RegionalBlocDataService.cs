namespace WPF_Project_Countries.Services
{
    using System;
    using System.Collections.Generic;
    using System.Data.SQLite;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    class RegionalBlocDataService
    {
        private SQLiteConnection connection;

        private SQLiteCommand command;

        private DialogService dialogService = new DialogService();

        public RegionalBlocDataService()
        {
            var path = @"Data\Countries.sqlite";

            try
            {
                connection = new SQLiteConnection("Data Source=" + path);
                connection.Open();

                string sqlcommand = "create table if not exists regionalBlocs(id int primary key, alpha3code char(3), acronym varchar(20), name varchar(50), foreign key(alpha3code) references country(alpha3code));" +
                    "create table if not exists otherAcronyms(id int primary key, id_regionalBloc int, otherAcronym varchar(50), foreign key (id_regionalBloc) references regionalBloc(id));" +
                    "create table if not exists otherNames(id int primary key, id_regionalBloc int, otherName varchar(50), foreign key (id_regionalBloc) references regionalBloc(id))";

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
