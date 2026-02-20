using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add Ocelot configuration
builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);

// Add services to the container
builder.Services.AddOcelot();
builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline
app.UseHttpsRedirection();

app.UseOcelot().Wait();

app.Run();
