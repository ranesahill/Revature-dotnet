using Microsoft.EntityFrameworkCore;

public class CrmDbContext : DbContext
{
    public CrmDbContext(DbContextOptions<CrmDbContext> options) : base(options) {}

    public DbSet<Customer> Customers { get; set; }
}