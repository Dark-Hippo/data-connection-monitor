using CsvHelper;
using CsvHelper.Configuration;
using DataConnectionMonitorAPI;
using System.Globalization;

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

var app = builder.Build();

var disconnectionsFile = app.Configuration["DisconnectionsFile"];
if (string.IsNullOrEmpty(disconnectionsFile))
{
    throw new InvalidOperationException("DisconnectionsFile is not set");
}

var lastSuccessfulConnectionFile = app.Configuration["LastSuccessfulConnectionFile"];
if (string.IsNullOrEmpty(lastSuccessfulConnectionFile))
{
    throw new InvalidOperationException("LastSuccessfulConnectionFile is not set");
}

var currentStatusFile = app.Configuration["CurrentStatusFile"];
if (string.IsNullOrEmpty(currentStatusFile))
{
    throw new InvalidOperationException("CurrentStatusFile is not set");
}

var success = int.TryParse(app.Configuration["Port"], out int port);
if (!success)
{
    throw new InvalidOperationException("Port is not set");
}

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

app.MapHub<DisconnectionsHub>("/last-ping").RequireCors("AllowAllOrigins");

app.MapGet("/disconnections", (DateTime? fromDate, DateTime? toDate) =>
{
    var disconnections = GetDisconnections(fromDate, toDate);
    return disconnections;
})
.WithName("GetDisconnections")
.WithOpenApi();

app.Run();

List<Disconnection> GetDisconnections(DateTime? fromDate, DateTime? toDate)
{
    // read data from connectionFailures.csv
    using var reader = new StreamReader(disconnectionsFile);
    var config = new CsvConfiguration(CultureInfo.InvariantCulture)
    {
        HasHeaderRecord = false,
        Delimiter = ","
    };
    using var csv = new CsvReader(reader, config);
    var disconnections = csv.GetRecords<Disconnection>().ToList();

    // filter by date
    if (fromDate.HasValue)
    {
        disconnections = disconnections.Where(d => d.Start >= fromDate).ToList();
    }
    if (toDate.HasValue)
    {
        disconnections = disconnections.Where(d => d.Start <= toDate).ToList();
    }

    return disconnections;
}