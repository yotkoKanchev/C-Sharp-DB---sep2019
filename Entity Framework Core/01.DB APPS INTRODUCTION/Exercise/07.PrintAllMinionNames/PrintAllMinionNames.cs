namespace P07PrintAllMinionNames
{
    using System;
    using System.Collections.Generic;
    using System.Data.SqlClient;

    public class PrintAllMinionNames
    {
        public static void Main()
        {
            var connectionString = @"Server=YOTO\SQLEXPRESS;Database=MinionsDB;Integrated Security=true";

            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var query = @"SELECT Name 
                                FROM Minions";
                var command = new SqlCommand(query, connection);
                var reader = command.ExecuteReader();
                var names = new List<string>();

                using (reader)
                {
                    while (reader.Read())
                    {
                        names.Add((string)reader["Name"]);
                    }
                }

                for (int i = 0; i < names.Count / 2; i++)
                {
                    Console.WriteLine(names[i]);
                    Console.WriteLine(names[names.Count - 1 - i]);
                }
            }
        }
    }
}
