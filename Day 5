namespace EmployeeDemo
{
    public class Employee
    {
        public int ID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Age { get; set; }

        public Employee(int ID, string FirstName, string LastName, int Age)
        {
            this.ID = ID;
            this.FirstName = FirstName;
            this.LastName = LastName;
            this.Age = Age;
        }

        public void Print()
        {
            Console.WriteLine($"ID: {this.ID}, Name: {this.FirstName} {this.LastName}, Age: {this.Age}");
        }
    }
}

namespace ExtensionMethodsDemo
{
    using EmployeeDemo;
    public static class EmployeeExtension 
    {
        public static void DoubleTheAge(this Employee x)
        {
            // x.Age *= 2;
            x.Age = x.Age * 2;
        }

    }
}
