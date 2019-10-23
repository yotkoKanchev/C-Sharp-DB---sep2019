namespace P04AddMinion
{
    using System;
    using System.Data.SqlClient;

    public class AddMinion
    {
        public static void Main()
        {
            var minionArgs = Console.ReadLine().Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var minionName = minionArgs[1];
            var minionAge = int.Parse(minionArgs[2]);
            var townName = minionArgs[3];

            var villainName = Console.ReadLine().Split(' ', StringSplitOptions.RemoveEmptyEntries)[1];

            var connectionString = @"Server=YOTO\SQLEXPRESS;Database=MinionsDB;Integrated Security=true";

            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();

                var transaction = connection.BeginTransaction();
                var command = new SqlCommand();

                command.Transaction = transaction;
                command.Connection = connection;
                command.Parameters.AddWithValue("@minionName", minionName);
                command.Parameters.AddWithValue("@minionAge", minionAge);
                command.Parameters.AddWithValue("@townName", townName);
                command.Parameters.AddWithValue("@villainName", villainName);

                try
                {
                    var townQuery = @"SELECT Id 
                                        FROM Towns 
                                       WHERE Name = @townName";

                    if (GetId(command, townQuery) == null)
                    {
                        var insertTownQuery = @"INSERT INTO Towns (Name) 
                                                VALUES (@townName)";

                        InsertData(command, insertTownQuery);
                        Console.WriteLine($"{townName} was added to the database.");
                    }

                    var townId = GetId(command, townQuery);
                    command.Parameters.AddWithValue("@townId", townId);

                    var villainQuery = @"SELECT Id 
                                           FROM Villains 
                                          WHERE Name = @villainName";

                    if (GetId(command, villainQuery) == null)
                    {
                        var insertVillainQuery = @"INSERT INTO Villains (Name) 
                                                   VALUES (@villainName)";

                        InsertData(command, insertVillainQuery);
                        Console.WriteLine($"Villain {villainName} was added to the database.");
                    }

                    var villainId = GetId(command, villainQuery);
                    command.Parameters.AddWithValue("@villainId", villainId);

                    var minionIdQuery = @"SELECT Id 
                                            FROM Minions 
                                           WHERE Name = @minionName AND Age = @minionAge";

                    if (GetId(command, minionIdQuery) == null)
                    {
                        var minionQuery = @"INSERT INTO Minions (Name, Age, TownId)
                                                 VALUES (@minionName, @minionAge, @townId)";
                        InsertData(command, minionQuery);
                    }

                    var minionId = GetId(command, minionIdQuery);
                    command.Parameters.AddWithValue("@minionId", minionId);

                    var villainMinionExistsQuery = @"SELECT TOP(1)     
                                                            FROM MinionsVillains
                                                           WHERE MinionId = @minionId AND VillainId = @villainId";

                    if (command.ExecuteScalar() == null)
                    {
                        var minionsVillainsQuery = @"INSERT INTO MinionsVillains (MinionId, VillainId) 
                                                          VALUES (@minionId, @villainId)";

                        InsertData(command, minionsVillainsQuery);
                        Console.WriteLine($"Successfully added {minionName} to be minion of {villainName}.");
                    }
                    else
                    {
                        Console.WriteLine($"{minionName} is already minion of {villainName}");
                    }

                    transaction.Commit();
                }
                catch (Exception ex1)
                {
                    try
                    {
                        Console.WriteLine(ex1.Message);
                        transaction.Rollback();
                    }
                    catch (Exception ex2)
                    {
                        Console.WriteLine(ex2.Message);
                    }
                }

            }
        }

        private static void InsertData(SqlCommand command, string query)
        {
            using (command)
            {
                command.CommandText = query;
                command.ExecuteNonQuery();
            }
        }

        private static object GetId(SqlCommand command, string query)
        {
            using (command)
            {
                command.CommandText = query;
                return command.ExecuteScalar();
            }
        }
    }
}
