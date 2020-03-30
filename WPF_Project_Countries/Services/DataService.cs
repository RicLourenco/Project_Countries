namespace WPF_Project_Countries.Services
{
    using Classes;
    using System;
    using System.Collections.Generic;
    using System.Data.SQLite;
    using System.IO;

    public class DataService
    {
        private SQLiteConnection connection;

        private SQLiteCommand command;

        private DialogService dialogService;

        public DataService()
        {
            if(!Directory.Exists("Data"))
            {
                Directory.CreateDirectory("Data");
            }

            var path = @"Data\Countries.sqlite";

            try
            {
                connection = new SQLiteConnection("Data Source=" + path);
                connection.Open();

                string sqlcommand = "create table if not exists countries(name varchar(50), alpha2code varchar(20), alpha3code varchar(20), capital varchar(50), region varchar(50), subregion varchar(50), population int, denonym varchar(50), area numeric, gini numeric, nativeName varchar(50), numericCode varchar(20) primary key, flag blob, cioc varchar(20))";

                command = new SQLiteCommand(sqlcommand, connection);

                command.ExecuteNonQuery();
            }
            catch(Exception e)
            {
                dialogService.ShowMessage("Erro", e.Message);
            }
        }

        public void SaveData(List<Country> countries)
        {
            try
            {
                foreach(var country in countries)
                {
                    string sql = string.Format($"insert into countries values('{country.Name}', '{country.Alpha2Code}', '{country.Alpha3Code}', '{country.Capital}', '{country.Region}', '{country.Subregion}', {country.Population}, '{country.Demonym}', '{country.Area}', '{country.Gini}', '{country.NativeName}', '{country.NumericCode}', {country.Flag}, '{country.Cioc}')");
                    
                    command = new SQLiteCommand(sql, connection);

                    command.ExecuteNonQuery();
                }

                connection.Close();
            }
            catch(Exception e)
            {
                dialogService.ShowMessage("Erro", e.Message);
            }
        }

        public List<Country> GetData()
        {
            List<Country> countries = new List<Country>();

            try
            {
                string sql = "select name, alpha2code, alpha3code, capital, region, subregion, population, denonym, area, gini, nativeName, numericCode, flag, cioc";

                command = new SQLiteCommand(sql, connection);

                SQLiteDataReader reader = command.ExecuteReader();

                while(reader.Read())
                {
                    countries.Add(new Country
                    {
                        Name = (string) reader["name"],
                        Alpha2Code = (string) reader["alpha2code"],
                        Alpha3Code = (string)reader["alpha3code"],
                        Capital = (string)reader["capital"],
                        Region = (string)reader["region"],
                        Subregion = (string)reader["subregion"],
                        Population = (long)reader["population"],
                        Demonym = (string)reader["denonym"],
                        Area = (double)reader["area"],
                        Gini = (double)reader["gini"],
                        NativeName = (string)reader["nativeName"],
                        NumericCode = (string)reader["numericCode"],
                        Flag = (Uri)reader["flag"],
                        Cioc = (string)reader["cioc"],
                    });
                }

                connection.Close();
                return countries;
            }
            catch(Exception e)
            {
                dialogService.ShowMessage("Erro", e.Message);
                return null;
            }
        }

        public void DeleteData()
        {
            try
            {
                string sql = "delete from countries";

                command = new SQLiteCommand(sql, connection);

                command.ExecuteNonQuery();
            }
            catch(Exception e)
            {
                dialogService.ShowMessage("Erro", e.Message);
            }
        }
    }
}
