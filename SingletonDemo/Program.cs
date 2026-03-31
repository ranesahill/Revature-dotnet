using Microsoft.Extensions.DependencyInjection;

var services = new ServiceCollection();
services.AddSingleton<DiSignleton>();

var serviceProvider = services.BuildServiceProvider();

var diLogger1 = serviceProvider.GetService<DiSignleton>();
var diLogger2 = serviceProvider.GetService<DiSignleton>();

var loggerManual = new DiSignleton();

Console.WriteLine(diLogger1.GetHashCode());
Console.WriteLine(diLogger2.GetHashCode());
Console.WriteLine(loggerManual.GetHashCode());

public class DiSignleton
{
    public int Value { get; set; }
}