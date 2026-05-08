var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services
       .AddReverseProxy()
       .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

builder.Services.AddHealthChecks();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapHealthChecks("/health/live");
app.MapReverseProxy();

app.Run();
