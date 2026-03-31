// using System;
// using Microsoft.Data.SqlClient;
// using Microsoft.VisualBasic;

// class Program
// {



//     static void Main()
//     {
//         string connectionString =
//         "Server=localhost,1433;Database=demodb;User Id=sa;Password=Sahil@12345;TrustServerCertificate=True;";

//         using var connection = new SqlConnection(connectionString);

//         try
//         {
//             connection.Open();
//             Console.WriteLine("Connected Successfully!");

//             SqlCommand command =
//                 new SqlCommand("SELECT * FROM customers", connection);

//             SqlDataReader reader = command.ExecuteReader();

//             while (reader.Read())
//             {
//                 Console.WriteLine($"{reader["id"]}  {reader["name"]}  {reader["age"]}");
//             }
//         }
//         catch (Exception ex)
//         {
//             Console.WriteLine(ex.Message);

//         }



//     }

//    static void ExecuteScalar(SqlConnection connection)
// {
//     var query = "SELECT COUNT(*) FROM Customers";

//     using (var command = new SqlCommand(query, connection))
//     {

//     var result = command.ExecuteScalar();

//     Console.WriteLine( result);}
// }

// }
