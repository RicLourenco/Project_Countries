namespace WPF_Project_Countries.Services
{
    using Library.Models;
    using System;
    using System.Collections.Generic;
    using System.Data.SQLite;
    using System.Drawing;
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

        public async Task SaveRegionalBlocAsync(List<RegionalBloc> regionalBlocs, string alpha3code)
        {
            await Task.Run(async () =>
            {
                foreach (RegionalBloc regionalBloc in regionalBlocs)
                {
                    command.Parameters.AddWithValue("@alpha3code", alpha3code);
                    command.Parameters.AddWithValue("@acronym", regionalBloc.Acronym);
                    command.Parameters.AddWithValue("@name", regionalBloc.Name);

                    command.CommandText = "insert into regionalBlocs values(null, @alpha3code, @acronym, @name)";

                    command.Connection = connection;

                    command.ExecuteNonQuery();

                    await otherAcronyms.SaveOtherAcronymsAsync(regionalBloc.OtherAcronyms);
                    await otherNames.SaveOtherNamesAsync(regionalBloc.OtherNames);
                }
            });

            //await Task.Run( async() => {
            //    foreach (RegionalBloc regionalBloc in regionalBlocs)
            //    {
            //        string sql = $"insert into regionalBlocs values(null, '{alpha3code}', '{regionalBloc.Acronym}', \"{regionalBloc.Name}\")";

            //        command = new SQLiteCommand(sql, connection);

            //        command.ExecuteNonQuery();

            //        await otherAcronyms.SaveOtherAcronymsAsync(regionalBloc.OtherAcronyms);
            //        await otherNames.SaveOtherNamesAsync(regionalBloc.OtherNames);
            //    }
            //});
        }

        public void GetRegionalBlocs(Country country)
        {
            try
            {
                string sql = $"select id, acronym, name from regionalBlocs where alpha3code = '{country.Alpha3Code}'";

                command = new SQLiteCommand(sql, connection);

                SQLiteDataReader reader = command.ExecuteReader();

                country.RegionalBlocs = new List<RegionalBloc>();

                while (reader.Read())
                {
                    country.RegionalBlocs.Add(new RegionalBloc
                    {
                        Acronym = reader["acronym"].ToString(),
                        Name = reader["name"].ToString(),
                        OtherAcronyms = new List<string>(otherAcronyms.GetOtherAcronyms(reader["id"].ToString())),
                        OtherNames = new List<string>(otherNames.GetOtherNames(reader["id"].ToString()))
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
