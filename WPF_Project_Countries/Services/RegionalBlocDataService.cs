namespace WPF_Project_Countries.Services
{
    using Library.Models;
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
        private DialogService dialogService;
        private OtherAcronymsService otherAcronyms;
        private OtherNamesService otherNames;

        public RegionalBlocDataService()
        {
            var path = @"Data\Countries.sqlite";

            try
            {
                connection = new SQLiteConnection("Data Source=" + path);
                connection.Open();

                string sqlcommand = "create table if not exists regionalBlocs(id integer primary key, alpha3code char(3), acronym varchar(20), name varchar(50), foreign key(alpha3code) references country(alpha3code))";

                command = new SQLiteCommand(sqlcommand, connection);

                command.ExecuteNonQuery();

                CreateOtherTables();
            }
            catch (Exception e)
            {
                dialogService.ShowMessage("Erro", e.Message);
            }
        }

        private void CreateOtherTables()
        {
            otherAcronyms = new OtherAcronymsService();
            otherNames = new OtherNamesService();
        }

        public async Task SaveRegionalBlocAsync(Country country)
        {
            await Task.Run(async () =>
            {
                foreach (RegionalBloc regionalBloc in country.RegionalBlocs)
                {
                    command.Parameters.AddWithValue("@alpha3code", country.Alpha3Code);
                    command.Parameters.AddWithValue("@acronym", regionalBloc.Acronym);
                    command.Parameters.AddWithValue("@name", regionalBloc.Name);

                    command.CommandText = "insert into regionalBlocs values(@alpha3code, @acronym, @name)";

                    command.Connection = connection;

                    command.ExecuteNonQuery();

                    //await otherAcronyms.SaveOtherAcronymsAsync(regionalBloc);
                }
            });
        }
    }
}
