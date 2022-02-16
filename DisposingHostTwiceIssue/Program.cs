using DisposingHostTwiceIssue;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHostedService<HostedService<A>>();
builder.Services.AddTransient<IMyLogger, DummyLogger>();

var app = builder.Build();
app.MapGet("/", () => "Hello World!");

await app.RunAsync();

public partial class Program { }
