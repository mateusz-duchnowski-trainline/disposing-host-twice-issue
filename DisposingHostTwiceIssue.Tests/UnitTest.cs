using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Xunit.Abstractions;

namespace DisposingHostTwiceIssue.Tests;

public class UnitTest : IClassFixture<MyWebApplicationFactory>
{
    private readonly WebApplicationFactory<Program> _webApplicationFactory;

    public UnitTest(MyWebApplicationFactory webApplicationFactory)
    {
        _webApplicationFactory = webApplicationFactory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                services.AddSingleton(webApplicationFactory.XunitMessageSink);
                services.AddTransient<IMyLogger, XUnitLogger>();
            });
        });
    }

    [Fact]
    public async Task Test1()
    {
        var response = await _webApplicationFactory.CreateClient()
            .SendAsync(new HttpRequestMessage(HttpMethod.Get, "/"));
        response.EnsureSuccessStatusCode();
    }
}

public class MyWebApplicationFactory : WebApplicationFactory<Program>
{
    // used only to inject the xunit IMessageSink and see log messages in test output
    public MyWebApplicationFactory(IMessageSink xunitMessageSink)
    {
        XunitMessageSink = xunitMessageSink;
    }

    public IMessageSink XunitMessageSink { get; }
}

public class XUnitLogger : IMyLogger
{
    private readonly IMessageSink _messageSink;

    public XUnitLogger(IMessageSink messageSink)
    {
        _messageSink = messageSink;
    }

    public void Log(string msg) => _messageSink.OnMessage(new TestMessage { Message = msg });
}

public class TestMessage : IDiagnosticMessage
{
    public string Message { get; set; }
}