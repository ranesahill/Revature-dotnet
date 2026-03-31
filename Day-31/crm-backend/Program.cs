using Microsoft.EntityFrameworkCore;
var builder = WebApplication.CreateBuilder(args);

// Services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors();

builder.Services.AddDbContext<CrmDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("CrmDb"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("CrmDb"))
    ));

var app = builder.Build();

// Middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors(options =>
    options.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

// ---------------- API ----------------

// Get all customers
app.MapGet("/api/customers", () =>
{
    return Results.Ok(new List<Customer>
    {
        new(1, "John Doe", "john@example.com", "Acme Inc."),
        new(2, "Jane Doe", "jane@example.com", "Acme Inc."),
        new(5, "Yash Sharma", "yash@gmail.com", "TCS"),
        new(6, "Rohit Kumar", "rohit@gmail.com", "Wipro"),
    });
});

// Get one customer
app.MapGet("/api/customers/{id}", (int id) =>
{
    return Results.Ok(new Customer(id, "John Doe", "john@example.com", "Acme Inc."));
});

// Create
app.MapPost("/api/customers", (Customer customer) =>
{
    return Results.Ok(customer);
});

// Update
app.MapPut("/api/customers/{id}", (int id, Customer customer) =>
{
    return Results.Ok(customer);
});

// Delete
app.MapDelete("/api/customers/{id}", (int id) =>
{
    return Results.Ok($"Deleted {id}");
});

app.Run();

// ---------------- MODEL ----------------

record Customer(int Id, string Name, string Email, string Company);