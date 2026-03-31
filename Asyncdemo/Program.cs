using System.Diagnostics;

try
{
    await DemonstrateExceptions();
}
catch (AggregateException ex)
{
    Console.WriteLine($"Aggregate Exception: {ex.Message}");
}


async Task DemonstrateExceptions()
{
    using var _client = new HttpClient();

    var urls = new[]
    {
        "https://jsonplaceholder.typicode.com/posts/1",   // valid
        "https://this-does-not-exist.invalid/post/1",       // will fail
        "https://this-does-not-exist.invalid/post/2",       // will fail
        "https://jsonplaceholder.typicode.com/posts/3"    // valid
    };

    var tasks = urls.Select(url => _client.GetStringAsync(url)).ToList();

    try
    {
        Console.WriteLine($"Count: {tasks.Count}");
        string[] results = await Task.WhenAll(tasks.ToArray());
       
        Console.WriteLine($"All {results.Length} succeeded.");
    }
    catch (HttpRequestException ex)
    {
        // Only the first exception is re-thrown by await
        Console.WriteLine($"At least one failed: {ex.Message}");

        // Inspect the Task directly for all exceptions
        foreach (var task in tasks.Where(t => t.IsFaulted))
        {
            Console.WriteLine($"  - {task.Exception?.InnerException?.Message}");
        }
    }

    foreach (var task in tasks)
    {

        if (task.IsCompletedSuccessfully)
        {
            Console.WriteLine($"Success: {task.Result.Length} chars");
        }
        else if (task.IsFaulted)
        {
            Console.WriteLine($"Failed: {task.Exception?.InnerException?.Message}");
        }
    }
}


