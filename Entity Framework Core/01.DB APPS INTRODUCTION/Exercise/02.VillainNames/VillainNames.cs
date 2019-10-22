namespace P02VillainNames
{
    using System;
    using System.Data.SqlClient;

    public class VillainNames
    {
        public static void Main()
        {
            string connectionString = @"Server=YOTO\SQLEXPRESS;Database=MinionsDB;Integrated Security=true";

            var connection = new SqlConnection(connectionString);

            connection.Open();

            using (connection)
            {
                var command = new SqlCommand(@"SELECT v.Name, COUNT(mv.VillainId) AS MinionsCount 
                                                FROM Villains AS v 
                                                JOIN MinionsVillains AS mv ON v.Id = mv.VillainId 
                                            GROUP BY v.Id, v.Name 
                                              HAVING COUNT(mv.VillainId) > 3 
                                            ORDER BY COUNT(mv.VillainId)", connection);
                var reader = command.ExecuteReader();

                using (reader)
                {
                    while (reader.Read())
                    {
                        string name = (string)reader["Name"];
                        int count = (int)reader["MinionsCount"];
                        Console.WriteLine($"{name} - {count}");
                    }
                }
            }
        }
    }
}
