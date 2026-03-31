using Microsoft.EntityFrameworkCore;
using WebApiDemo.Models;
using WebApiDemo.Services;      // if your service is inside Services folder
using WebApiDemo.Mappings;      // if your AutoMapper profile is here

var builder = WebApplication.CreateBuilder(args);

// Add Controllers
builder.Services.AddControllers();

// Add DbContext
builder.Services.AddDbContext<CrmDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("CrmDbConnection"),
        ServerVersion.AutoDetect(
            builder.Configuration.GetConnectionString("CrmDbConnection")
        )
    )
);

// Register Services
builder.Services.AddScoped<ICustomerService, CustomerService>();

// Register AutoMapper
builder.Services.AddAutoMapper(typeof(CustomerProfile));

var app = builder.Build();

app.MapControllers();

app.Run();