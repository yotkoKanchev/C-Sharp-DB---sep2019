namespace P09IncreaseAgeStoredProcedure
{
    using System;
    using System.Data.SqlClient;

    public class IncreaseAgeStoredProcedure
    {
        public static void Main()
        {
            var connectionString = @"Server=YOTO\SQLEXPRESS;Database=MinionsDB;Integrated Security=true";

            int minionId = int.Parse(Console.ReadLine());

            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();

                var query = @"EXECUTE dbo.usp_GetOlder @minionId";
                var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@minionId", minionId);

                using (command)
                {
                    command.ExecuteNonQuery();
                }

                var printQuery = @"SELECT Name, Age 
                                     FROM Minions 
                                    WHERE Id = @minionId";

                command.CommandText = printQuery;

                var reader = command.ExecuteReader();

                using (reader)
                {
                    if (reader.HasRows)
                    {
                        reader.Read();
                        var name = reader["Name"];
                        var age = reader["Age"];
                        Console.WriteLine($"{name} - {age} years old");
                    }
                }
            }
        }
    }
}
