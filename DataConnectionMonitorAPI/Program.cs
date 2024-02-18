using DataConnectionMonitorAPI;
using Microsoft.Extensions.Options;

var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{environmentName}.json", optional: true)
    .AddEnvironmentVariables();

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(options => // Add CORS services
{
    options.AddPolicy("AllowAllOrigins", builder =>
    {
        builder.WithOrigins("*")
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});
builder.Services.AddSignalR();
builder.Services.AddHostedService<LastSuccessfulConnectionService>();
builder.Services.AddHostedService<CurrentConnectionStatusService>();
builder.Services.Configure<Config>(builder.Configuration.GetSection("Config"));
builder.Services.AddControllers();

var app = builder.Build();

app.UseRouting()
.UseCors("AllowAllOrigins")
.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

var configuration = app.Services.GetRequiredService<IOptions<Config>>().Value;

var port = configuration.Port;

app.Urls.Add($"http://*:{port}");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// app.UseHttpsRedirection();

// Use CORS middleware
app.UseCors("AllowAllOrigins");

app.MapHub<DisconnectionsHub>("/disconnections-hub").RequireCors("AllowAllOrigins");

app.Run();
