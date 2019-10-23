namespace P06RemoveVillain
{
    using System;
    using System.Data.SqlClient;

    public class RemoveVillain
    {
        public static void Main()
        {
            var villainId = int.Parse(Console.ReadLine());

            var connectionString = @"Server=YOTO\SQLEXPRESS;Database=MinionsDB;Integrated Security=true";

            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();

                var transaction = connection.BeginTransaction();
                var command = new SqlCommand();
                command.Transaction = transaction;
                command.Connection = connection;
                command.Parameters.AddWithValue("@villainId", villainId);

                try
                {
                    var villainNameQuery = @"SELECT Name 
                                               FROM Villains 
                                              WHERE Id = @villainId";

                    var villainName = GetName(command, villainNameQuery);

                    if (villainName == null)
                    {
                        Console.WriteLine("No such villain was found.");
                        return;
                    }

                    var minionsVillainsQuery = @"DELETE 
                                                   FROM MinionsVillains 
                                                  WHERE VillainId = @villainId";

                    var releasedMinionsCount = DeleteRecords(command, minionsVillainsQuery);

                    var villainQuery = @"DELETE 
                                           FROM Villains 
                                          WHERE Id = @villainId";

                    DeleteRecords(command, villainQuery);

                    Console.WriteLine($"{(string)villainName} was deleted.");
                    Console.WriteLine($"{releasedMinionsCount} minions were released.");

                    transaction.Commit();
                }
                catch (Exception)
                {
                    try
                    {
                        transaction.Rollback();
                        throw new Exception("Database was not affected !!!");
                    }
                    catch (Exception)
                    {
                        throw new Exception("Transaction Rollback failed !!!");
                    }
                }

            }
        }

        private static object GetName(SqlCommand command, string query)
        {
            using (command)
            {
                command.CommandText = query;
                return command.ExecuteScalar();
            }
        }

        private static int DeleteRecords(SqlCommand command, string query)
        {
            using (command)
            {
                command.CommandText = query;
                return (int)command.ExecuteNonQuery();
            }
        }
    }
}
