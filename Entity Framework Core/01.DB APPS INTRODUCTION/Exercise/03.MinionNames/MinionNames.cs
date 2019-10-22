namespace P03MinionNames
{
    using System;
    using System.Data.SqlClient;

    public class MinionNames
    {
        public static void Main()
        {
            var connectionString = @"Server=YOTO\SQLEXPRESS;Database=MinionsDB;Integrated Security=true";

            var connection = new SqlConnection(connectionString);

            var vilianId = int.Parse(Console.ReadLine());

            connection.Open();

            using (connection)
            {

                var command = new SqlCommand(@"SELECT Name 
                                                 FROM Villains 
                                                WHERE Id = @villianId");

                command.Connection = connection;
                command.Parameters.AddWithValue("@villianId", vilianId);
                var villianName = (string)command.ExecuteScalar();

                if (villianName == null)
                {
                    Console.WriteLine($"No villain with ID {vilianId} exists in the database.");
                    return;
                }

                Console.WriteLine($"Villain: {villianName}");

                var minionsQuery = @"SELECT ROW_NUMBER() OVER (ORDER BY m.Name) as RowNum,
                                            m.Name, 
                                            m.Age
                                       FROM MinionsVillains AS mv
                                       JOIN Minions As m ON mv.MinionId = m.Id
                                      WHERE mv.VillainId = @villianId
                                   ORDER BY m.Name";

                command.CommandText = minionsQuery;

                var reader = command.ExecuteReader();

                using (reader)
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            var minionName = reader["Name"];
                            var minionAge = reader["Age"];
                            var rowNum = reader["RowNum"];
                            Console.WriteLine($"{rowNum}. {minionName} {minionAge}");
                        }
                    }
                    else
                    {
                        Console.WriteLine("(no minions)");
                    }
                }
            }
        }
    }
}
