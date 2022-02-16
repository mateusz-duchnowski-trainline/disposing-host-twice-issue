namespace DisposingHostTwiceIssue;

public class HostedService<Tag> : IHostedService, IAsyncDisposable
{
    private readonly IMyLogger _logger;

    public HostedService(IMyLogger logger)
    {
        _logger = logger;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        Log("started");
        return Task.CompletedTask;
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        Log("stopping");
        await SomeDelay();
        Log("stopped");
    }

    public async ValueTask DisposeAsync()
    {
        Log("disposing");
        await SomeDelay();
        Log("disposed");
    }

    private void Log(string msg) => _logger.Log($"{typeof(Tag).Name} {msg}");

    private static Task SomeDelay()
    {
        return Task.Delay(TimeSpan.FromMilliseconds(200));
    }
}

public interface IMyLogger
{
    void Log(string msg);
}

public class DummyLogger : IMyLogger
{
    public void Log(string msg) { }
}

// to easily register multiple HostedServices
struct A { }
struct B { }
struct C { }
