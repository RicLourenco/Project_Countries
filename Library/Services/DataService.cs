namespace Library.Services
{
    using System;
    using System.Collections.Generic;
    using System.Data.SQLite;
    using System.IO;
    using Models;

    public class DataService
    {
        private SQLiteConnection connection;

        private SQLiteCommand command;

        private DialogService dialogService;

        /// <summary>
        /// Default constructor that creates a new local sqlite table for the countries, if one doesn't exist
        /// </summary>
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

                string sqlcommand = "create table if not exists countries(name varchar(50), alpha2code varchar(20), alpha3code varchar(20) primary key, capital varchar(50), region varchar(50), subregion varchar(50), population int, denonym varchar(50), area numeric, gini numeric, nativeName varchar(50), numericCode varchar(20), flag blob, cioc varchar(20))";

                command = new SQLiteCommand(sqlcommand, connection);

                command.ExecuteNonQuery();
            }
            catch(Exception e)
            {
                dialogService.ShowMessage("Erro", e.Message);
            }
        }

        /// <summary>
        /// Receive a C# list of countries and insert into the countries sqlite table
        /// </summary>
        /// <param name="countries"></param>
        public void SaveData(List<Country> countries)
        {
            try
            {
                foreach(var country in countries)
                {
                    command.Parameters.AddWithValue("@name", country.Name);
                    command.Parameters.AddWithValue("@alpha2code", country.Alpha2Code);
                    command.Parameters.AddWithValue("@alpha3code", country.Alpha3Code);
                    command.Parameters.AddWithValue("@capital", country.Capital);
                    command.Parameters.AddWithValue("@region", country.Region);
                    command.Parameters.AddWithValue("@subregion", country.Subregion);
                    command.Parameters.AddWithValue("@population", country.Population);
                    command.Parameters.AddWithValue("@denonym", country.Demonym);
                    command.Parameters.AddWithValue("@area", country.Area);
                    command.Parameters.AddWithValue("@gini", country.Gini);
                    command.Parameters.AddWithValue("@nativeName", country.NativeName);
                    command.Parameters.AddWithValue("@numericCode", country.NumericCode);
                    command.Parameters.AddWithValue("@flag", country.Flag);
                    command.Parameters.AddWithValue("@cioc", country.Cioc);

                    command.CommandText = "insert into countries values(@name, @alpha2code, @alpha3code, @capital, @region, @subregion, @population, @denonym, @area, @gini, @nativeName, @numericCode, @flag, @cioc)";

                    command.Connection = connection;

                    command.ExecuteNonQuery();

                    /*
                    string sql = string.Format($"insert into countries values('{country.Name}', '{country.Alpha2Code}', '{country.Alpha3Code}', '{country.Capital}', '{country.Region}', '{country.Subregion}', {country.Population}, '{country.Demonym}', '{country.Area}', '{country.Gini}', '{country.NativeName}', '{country.NumericCode}', {country.Flag}, '{country.Cioc}')");
                    
                    command = new SQLiteCommand(sql, connection);
                    */
                }

                connection.Close();
            }
            catch(Exception e)
            {
                dialogService.ShowMessage("Erro", e.Message);
            }
        }

        /// <summary>
        /// Read all rows from the countries sqlite table and insert into the countries C# list
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// Delete all rows from the contries sqlite table
        /// </summary>
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
