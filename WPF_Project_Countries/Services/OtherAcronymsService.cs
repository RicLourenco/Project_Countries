namespace WPF_Project_Countries.Services
{
    #region Usings

    using System;
    using System.Collections.Generic;
    using System.Data.SQLite;
    using System.Threading.Tasks;

    #endregion

    class OtherAcronymsService
    {
        #region Variables

        private SQLiteCommand command;

        private readonly DialogService dialogService = new DialogService();

        #endregion

        #region Constructor

        /// <summary>
        /// Parametrized constructor for the other acronyms services, that receives an SQLiteConnection and creates an SQLite table for the OtherAcronyms string list
        /// </summary>
        /// <param name="connection"></param>
        public OtherAcronymsService(SQLiteConnection connection)
        {
            try
            {
                string sqlcommand = "create table if not exists otherAcronyms(id_regionalBloc int, otherAcronym varchar(50), foreign key (id_regionalBloc) references regionalBloc(id));";

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
        /// Gets the otherAcronyms strings from the sqlite table otherAcronyms and adds them to a regionaBloc's otherAcronyms list
        /// </summary>
        /// <param name="id"></param>
        /// <param name="connection"></param>
        /// <returns></returns>
        public async Task<List<string>> GetOtherAcronyms(string id, SQLiteConnection connection)
        {
            try
            {
                List<string> otherAcronyms = new List<string>();

                await Task.Run(() =>
                {
                    string sql = $"select otherAcronym from otherAcronyms where id_regionalBloc = '{id}'";

                    command = new SQLiteCommand(sql, connection);

                    SQLiteDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        otherAcronyms.Add(reader["otherAcronym"].ToString());
                    }
                });
                return otherAcronyms;
            }
            catch (Exception e)
            {
                dialogService.ShowMessage("Error", e.Message);
                return null;
            }
        }

        /// <summary>
        /// Receives a regional bloc's list of otherAcronyms string, and inserts it into the otherAcronyms sqlite table using an SQLiteConnection
        /// </summary>
        /// <param name="otherAcronyms"></param>
        /// <param name="connection"></param>
        /// <returns></returns>
        public async Task SaveOtherAcronymsAsync(List<string> otherAcronyms, SQLiteConnection connection)
        {
            try
            {
                foreach (string otherAcronym in otherAcronyms)
                {
                    command.Parameters.AddWithValue("@otherAcronym", otherAcronym);

                    command.CommandText = "insert into otherAcronyms values((select id from regionalBlocs order by id desc limit 1), @otherAcronym)";

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