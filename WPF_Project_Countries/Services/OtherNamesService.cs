namespace WPF_Project_Countries.Services
{
    #region Usings

    using System;
    using System.Collections.Generic;
    using System.Data.SQLite;
    using System.Threading.Tasks;

    #endregion

    class OtherNamesService
    {
        #region Variables

        private SQLiteCommand command;

        private readonly DialogService dialogService = new DialogService();

        #endregion

        #region Constructor

        /// <summary>
        /// Parametrized constructor for the other names services, that receives an SQLiteConnection and creates an SQLite table for the OtherNames string list
        /// </summary>
        /// <param name="connection"></param>
        public OtherNamesService(SQLiteConnection connection)
        {
            try
            {
                string sqlcommand = "create table if not exists otherNames(id_regionalBloc integer, otherName varchar(50), foreign key (id_regionalBloc) references regionalBloc(id))";

                command = new SQLiteCommand(sqlcommand, connection);

                command.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                dialogService.ShowMessage("Error", e.Message);
            }
        }

        #endregion

        #region Methods (alphabetical order)

        /// <summary>
        /// Gets the otherNames strings from the sqlite table otherNames and adds them to a regionaBloc's otherNames list
        /// </summary>
        /// <param name="id"></param>
        /// <param name="connection"></param>
        /// <returns></returns>
        public async Task<List<string>> GetOtherNames(string id, SQLiteConnection connection)
        {
            try
            {
                List<string> otherNames = new List<string>();

                await Task.Run(() =>
                {
                    string sql = $"select otherName from otherNames where id_regionalBloc = '{id}'";

                    command = new SQLiteCommand(sql, connection);

                    SQLiteDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        otherNames.Add(reader["otherName"].ToString());
                    }
                });

                return otherNames;
            }
            catch (Exception e)
            {
                dialogService.ShowMessage("Error", e.Message);
                return null;
            }
        }

        /// <summary>
        /// Receives a regional bloc's list of otherNames string, and inserts it into the otherNames sqlite table using an SQLiteConnection
        /// </summary>
        /// <param name="otherNames"></param>
        /// <param name="connection"></param>
        /// <returns></returns>
        public async Task SaveOtherNamesAsync(List<string> otherNames, SQLiteConnection connection)
        {
            try
            {
                foreach (string otherName in otherNames)
                {
                    command.Parameters.AddWithValue("@otherName", otherName);

                    command.CommandText = "insert into otherNames values((select id from regionalBlocs order by id desc limit 1), @otherName)";

                    command.Connection = connection;

                    await command.ExecuteNonQueryAsync();
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