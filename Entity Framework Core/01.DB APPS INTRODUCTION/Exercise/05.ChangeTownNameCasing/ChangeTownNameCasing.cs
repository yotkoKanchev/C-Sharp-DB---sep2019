namespace ChangeTownNameCasing
{
    using System;
    using System.Collections.Generic;
    using System.Data.SqlClient;

    public class ChangeTownNameCasing
    {
        public static void Main()
        {
            var countryName = Console.ReadLine();

            var connectionString = @"Server=YOTO\SQLEXPRESS;Database=MinionsDB;Integrated Security=true";

            var connection = new SqlConnection(connectionString);

            connection.Open();

            using (connection)
            {
                var command = new SqlCommand();
                command.Connection = connection;
                command.Parameters.AddWithValue("@countryName", countryName);

                var rowsAffected = UpdateTowns(command);

                if ((int)rowsAffected > 0)
                {
                    Console.WriteLine($"{rowsAffected} town names were affected.");
                    PrintTowns(command);
                }
                else
                {
                    Console.WriteLine("No town names were affected.");
                }
            }
        }

        private static void PrintTowns(SqlCommand command)
        {
            var query = @"SELECT Name 
                            FROM Towns
                           WHERE CountryCode = (SELECT Id 
                                                  FROM Countries 
                                                 WHERE Name = @countryName)";
            using (command)
            {
                command.CommandText = query;
                var reader = command.ExecuteReader();

                var towns = new List<string>();

                using (reader)
                {
                    while (reader.Read())
                    {
                        towns.Add((string)reader["Name"]);
                    }
                }

                Console.WriteLine($"[{string.Join(", ", towns)}]");
            }
            
        }

        private static int UpdateTowns(SqlCommand command)
        {
            var query = @"UPDATE Towns
                                 SET Name = UPPER(Name)
                               WHERE CountryCode = (SELECT Id 
                                                      FROM Countries 
                                                     WHERE Name = @countryName)";
            using (command)
            {
                command.CommandText = query;
                return (int)command.ExecuteNonQuery();
            }
        }
    }
}
