using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
 
namespace WebApiDemo.Models;

    // =========================
    // DbContext
    // =========================
    public class CrmDbContext : DbContext
    {
        public CrmDbContext(DbContextOptions<CrmDbContext> options)
            : base(options)
        {
        }
 
        public DbSet<Customer> Customers { get; set; }
    }
 
 
    // =========================
    // Model
    // =========================
    public class Customer
    {
        public required int Id { get; set; }
        public required string Name { get; set; }
        public required string Email { get; set; }
    }
 
 
    // =========================
    // Interface
    // =========================
    public interface ICustomerService
    {
        IEnumerable<Customer> GetAllCustomers();
        Customer AddCustomer(Customer customer);
    }
 
 
    // =========================
    // Service
    // =========================
    public class CustomerService : ICustomerService
    {
        private readonly CrmDbContext dbContext;
 
        public CustomerService(CrmDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
 
        public IEnumerable<Customer> GetAllCustomers()
        {
            return dbContext.Customers.ToList();
        }
 
        public Customer AddCustomer(Customer customer)
        {
            dbContext.Customers.Add(customer);
            dbContext.SaveChanges();
            return customer;
        }
    }
