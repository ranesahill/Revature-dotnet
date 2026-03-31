using Consul;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddSingleton<IConsulClient, ConsulClient>(p =>
    new ConsulClient(cfg =>
    {
        cfg.Address = new Uri("http://localhost:8500");
    }));

var app = builder.Build();

app.MapControllers();

var consul = app.Services.GetRequiredService<IConsulClient>();

var registration = new AgentServiceRegistration()
{
    ID = $"order-service-{Guid.NewGuid()}",
    Name = "order-service",
    Address = "localhost",
    Port = 7000
};

await consul.Agent.ServiceRegister(registration);

app.Lifetime.ApplicationStopping.Register(() =>
{
    consul.Agent.ServiceDeregister(registration.ID).Wait();
});

app.Run("http://localhost:7000");