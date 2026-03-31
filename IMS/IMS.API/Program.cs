using System.Text;
using IMS.App.Data;
using IMS.App.Handlers;
using IMS.App.Services;
using IMS.Core.Interfaces;
using IMS.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// ── Database (MySQL) ────────────────────────────────────────────
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

// ── Redis Cache ─────────────────────────────────────────────────
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration["Redis:ConnectionString"];
    options.InstanceName = "IMS_";
});

// ── JWT Authentication ──────────────────────────────────────────
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
        };
    });
builder.Services.AddAuthorization();

// ── Repositories ────────────────────────────────────────────────
builder.Services.AddScoped<IInvoiceRepository, InvoiceRepository>();
builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
builder.Services.AddScoped<IInvoiceLineItemRepository, InvoiceLineItemRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

// ── Services ────────────────────────────────────────────────────
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<ICacheService, CacheService>();
builder.Services.AddScoped<IInvoiceNumberGenerator, InvoiceNumberGenerator>();

// ── CQRS Handlers ───────────────────────────────────────────────
builder.Services.AddScoped<CreateInvoiceHandler>();
builder.Services.AddScoped<UpdateInvoiceHandler>();
builder.Services.AddScoped<DeleteInvoiceHandler>();
builder.Services.AddScoped<AddInvoiceLineItemHandler>();
builder.Services.AddScoped<UpdateInvoiceLineItemHandler>();
builder.Services.AddScoped<DeleteInvoiceLineItemHandler>();
builder.Services.AddScoped<ApplyPaymentHandler>();
builder.Services.AddScoped<ChangeInvoiceStatusHandler>();
builder.Services.AddScoped<GetInvoiceByIdHandler>();
builder.Services.AddScoped<GetAllInvoicesHandler>();
builder.Services.AddScoped<GetAgingReportHandler>();
builder.Services.AddScoped<GetInvoiceAnalyticsHandler>();
builder.Services.AddScoped<GetDsoHandler>();
builder.Services.AddScoped<GetOutstandingHandler>();

// ── Controllers ─────────────────────────────────────────────────
builder.Services.AddControllers();

// ── Swagger ─────────────────────────────────────────────────────
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Invoice Management System API",
        Version = "v1",
        Description = "A secure financial invoicing and payment reconciliation API"
    });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token.",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// ── CORS ────────────────────────────────────────────────────────
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
    {
        policy.WithOrigins("http://localhost:5173", "http://localhost:3000")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

// ── Middleware Pipeline ─────────────────────────────────────────
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowReactApp");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// ── Auto-migrate on startup (development) ───────────────────────
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();

    // ── Force-Reset Demo Passwords ──────────────────────────
    var newHash = BCrypt.Net.BCrypt.HashPassword("Password123!");
    await db.Database.ExecuteSqlRawAsync("UPDATE Users SET PasswordHash = {0}", newHash);
}

app.Run();