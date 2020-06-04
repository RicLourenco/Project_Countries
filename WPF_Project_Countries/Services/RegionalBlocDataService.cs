namespace WPF_Project_Countries.Services
{
    #region Usings

    using System;
    using System.Collections.Generic;
    using System.Data.SQLite;
    using System.Threading.Tasks;
    using Library.Models;

    #endregion

    class RegionalBlocDataService
    {
        #region Variables

        private SQLiteCommand command;

        private readonly DialogService dialogService = new DialogService();

        private OtherAcronymsService otherAcronyms;

        private OtherNamesService otherNames;

        #endregion

        #region Constructor

        /// <summary>
        /// Parametrized constructor for the regional blocs services, that receives an SQLiteConnection and creates an SQLite table for the RegionalBloc object
        /// </summary>
        /// <param name="connection"></param>
        public RegionalBlocDataService(SQLiteConnection connection)
        {
            try
            {
                string sqlcommand = "create table if not exists regionalBlocs(id integer primary key, alpha3code char(3), acronym varchar(20), name varchar(50), foreign key(alpha3code) references country(alpha3code))";

                command = new SQLiteCommand(sqlcommand, connection);

                command.ExecuteNonQuery();

                CreateOtherTables(connection);
            }
            catch (Exception e)
            {
                dialogService.ShowMessage("Error", e.Message);
            }
        }

        #endregion

        #region Methods (alphabetical order)

        /// <summary>
        /// Calls the otherAcronyms and otherNnames services constructors to create a new sqlite table for each
        /// </summary>
        /// <param name="connection"></param>
        private void CreateOtherTables(SQLiteConnection connection)
        {
            otherAcronyms = new OtherAcronymsService(connection);
            otherNames = new OtherNamesService(connection);
        }

        /// <summary>
        /// Gets the regionalBlocs objects from the sqlite table regionalBlocs and adds them to a country's regionalBlocs list
        /// </summary>
        /// <param name="country"></param>
        /// <param name="connection"></param>
        /// <returns></returns>
        public async Task GetRegionalBlocs(Country country, SQLiteConnection connection)
        {
            try
            {
                await Task.Run(async () =>
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
                            OtherAcronyms = new List<string>(await otherAcronyms.GetOtherAcronyms(reader["id"].ToString(), connection)),
                            OtherNames = new List<string>(await otherNames.GetOtherNames(reader["id"].ToString(), connection))
                        });
                    }
                });
            }
            catch (Exception e)
            {
                dialogService.ShowMessage("Error", e.Message);
            }
        }

        /// <summary>
        /// Receives a country's alpha3code and its respective list of regionalBlocs object, and inserts it into the regionalBlocs sqlite table using an SQLiteConnection
        /// </summary>
        /// <param name="regionalBlocs"></param>
        /// <param name="alpha3code"></param>
        /// <param name="connection"></param>
        /// <returns></returns>
        public async Task SaveRegionalBlocAsync(List<RegionalBloc> regionalBlocs, string alpha3code, SQLiteConnection connection)
        {
            try
            {
                foreach (RegionalBloc regionalBloc in regionalBlocs)
                {
                    command.Parameters.AddWithValue("@alpha3code", alpha3code);
                    command.Parameters.AddWithValue("@acronym", regionalBloc.Acronym);
                    command.Parameters.AddWithValue("@name", regionalBloc.Name);

                    command.CommandText = "insert into regionalBlocs values(null, @alpha3code, @acronym, @name)";

                    command.Connection = connection;

                    await command.ExecuteNonQueryAsync();

                    await otherAcronyms.SaveOtherAcronymsAsync(regionalBloc.OtherAcronyms, connection);
                    await otherNames.SaveOtherNamesAsync(regionalBloc.OtherNames, connection);
                }
            }
            catch (Exception e)
            {
                dialogService.ShowMessage("Error", e.Message);
            }
        }

        #endregion
    }
}