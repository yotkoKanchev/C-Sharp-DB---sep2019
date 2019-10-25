namespace P08IncreaseMinionAge
{
    using System;
    using System.Data.SqlClient;
    using System.Linq;

    public class IncreaseMinionAge
    {
        public static void Main()
        {
            var connectionString = @"Server=YOTO\SQLEXPRESS;Database=MinionsDB;Integrated Security=true";

            var minionIds = Console.ReadLine()
                .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .Select(int.Parse)
                .ToArray();

            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();

                foreach (var id in minionIds)
                {
                    var query = @"UPDATE Minions 
                                     SET Age += 1 , Name = UPPER(LEFT(Name, 1))+LOWER(SUBSTRING(Name, 2, LEN(Name))) 
                                   WHERE Id = @minionId";
                    var command = new SqlCommand(query, connection);

                    using (command)
                    {
                        command.Parameters.AddWithValue("@minionId", id);
                        command.ExecuteNonQuery();
                    }
                }

                var readQuery = @"SELECT Name, Age FROM Minions";

                using (var readCommand = new SqlCommand(readQuery, connection))
                {
                    var reader = readCommand.ExecuteReader();

                    using (reader)
                    {
                        while (reader.Read())
                        {
                            var name = reader["Name"];
                            var age = reader["Age"];
                            Console.WriteLine($"{name} {age}");
                        }
                    }
                }
            }
        }
    }
}
