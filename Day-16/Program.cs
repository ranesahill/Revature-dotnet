
// using System;
// using Microsoft.Data.SqlClient;

// class Program
// {
//     static void Main()
//     {
//         string connectionString =
//         "Server=localhost,1433;Database=demodb;User Id=sa;Password=Sahil@12345;TrustServerCertificate=True;";

//         using (SqlConnection connection = new SqlConnection(connectionString))
//         {
//             try
//             {
//                 connection.Open();
//                 Console.WriteLine("Connected Successfully!");

//                 //ExecuteScalarExample(connection);
//                 ExecuteReaderExample(connection);
//             }
//             catch (Exception ex)
//             {
//                 Console.WriteLine(ex.Message);
//             }
//         }
//     }

//     static void ExecuteScalarExample(SqlConnection connection)
//     {
//         string query = "SELECT COUNT(*) FROM customers";

//         using (SqlCommand command = new SqlCommand(query, connection))
//         {
//             var result = command.ExecuteScalar();
//             Console.WriteLine("Total Customers: " + result);
//         }
//     }

//     static void ExecuteReaderExample(SqlConnection connection)
//     {
//         string query = "SELECT id, name, age FROM customers";

//         using (SqlCommand command = new SqlCommand(query, connection))
//         using (SqlDataReader reader = command.ExecuteReader())
//         {
//             Console.WriteLine("Customer List:");

//             while (reader.Read())
//             {
//                 Console.WriteLine($"{reader["id"]} {reader["name"]} {reader["age"]}");
//             }
//         }
//     }
// }


using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;

// Model (Table)
public class Student
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public int Age { get; set; }

     public string City { get; set; } = "";
}

// DbContext (Database connection)
public class StudentContext : DbContext
{
    public DbSet<Student> Students { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseMySql(
    "server=localhost;database=studentdb;user=root;password=sahil123",
    new MySqlServerVersion(new Version(8,0,36))
);

    }
}

class Program
{
    static void Main()
    {
        using (var context = new StudentContext())
        {
            // Insert student
    //    context.Students.Add(new Student { Name = "Rahul", Age = 20, City = "Pune" });

    //         context.SaveChanges();

            // Update student
            var student = context.Students.FirstOrDefault(s => s.Name == "Rahul");
            if (student != null)
            {
                student.Age = 22;
                context.SaveChanges();
            }

            // Display all students
            var students = context.Students.ToList();

            foreach (var s in students)
            {
                Console.WriteLine($"Id: {s.Id}, Name: {s.Name}, Age: {s.Age}, City: {s.City}");
            }
        }
    }
}



