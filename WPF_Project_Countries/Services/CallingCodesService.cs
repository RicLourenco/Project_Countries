namespace WPF_Project_Countries.Services
{
    using Library.Models;
    using System;
    using System.Collections.Generic;
    using System.Data.SQLite;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    class CallingCodesService
    {
        private SQLiteConnection connection;

        private SQLiteCommand command;

        private DialogService dialogService = new DialogService();

        public CallingCodesService()
        {
            var path = @"Data\Countries.sqlite";

            try
            {
                connection = new SQLiteConnection("Data Source=" + path);
                connection.Open();

                string sqlcommand = "create table if not exists callingCodes(alpha3code char(3), callingCode varchar(10), foreign key (alpha3code) references countries(alpha3code));";

                command = new SQLiteCommand(sqlcommand, connection);

                command.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                dialogService.ShowMessage("Erro", e.Message);
            }
        }

        public async Task SaveCallingCodesAsync(List<string> callingCodes, string alpha3code)
        {
            await Task.Run(() =>
            {
                foreach (string callingCode in callingCodes)
                {
                    command.Parameters.AddWithValue("@alpha3code", alpha3code);
                    command.Parameters.AddWithValue("@callingCode", callingCode);

                    command.CommandText = "insert into callingCodes values(@alpha3code, @callingCode)";

                    command.Connection = connection;

                    command.ExecuteNonQuery();
                }
            });

            //await Task.Run(() => {
            //    foreach (string callingCode in callingCodes)
            //    {
            //        string sql = $"insert into callingCodes values('{alpha3code}', '{callingCode}')";

            //        command = new SQLiteCommand(sql, connection);

            //        command.ExecuteNonQuery();
            //    }
            //});
        }

        public void GetCallingCodes(Country country)
        {
            try
            {
                string sql = $"select alpha3code, callingCode from callingCodes where alpha3code = '{country.Alpha3Code}'";

                command = new SQLiteCommand(sql, connection);

                SQLiteDataReader reader = command.ExecuteReader();

                country.CallingCodes = new List<string>();

                while (reader.Read())
                {
                    country.CallingCodes.Add(reader["callingCode"].ToString());
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
