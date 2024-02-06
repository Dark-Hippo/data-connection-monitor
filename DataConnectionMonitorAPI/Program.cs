using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;

var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{environmentName}.json", optional: true)
    .AddEnvironmentVariables();

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer()
    .AddSwaggerGen()
    .AddCors(options => // Add CORS services
    {
        options.AddPolicy("AllowAllOrigins", builder =>
        {
            builder.WithOrigins("*")
                .AllowAnyMethod()
                .AllowAnyHeader();
        });
    });

var port = Environment.GetEnvironmentVariable("PORT") ?? "5000";

builder.WebHost.UseUrls($"http://*:{port}");

var app = builder.Build();


var disconnectionsFile = app.Configuration["DisconnectionsFile"];
if(string.IsNullOrEmpty(disconnectionsFile))
{
    throw new InvalidOperationException("DisconnectionsFile is not set");
}

var lastSuccessfulConnectionFile = app.Configuration["LastSuccessfulConnectionFile"];
if(string.IsNullOrEmpty(lastSuccessfulConnectionFile))
{
    throw new InvalidOperationException("LastSuccessfulConnectionFile is not set");
}

var currentStatusFile = app.Configuration["CurrentStatusFile"];
if(string.IsNullOrEmpty(currentStatusFile))
{
    throw new InvalidOperationException("CurrentStatusFile is not set");
}


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    
}

app.UseHttpsRedirection();

// Use CORS middleware
app.UseCors("AllowAllOrigins");

app.MapGet("/disconnections", (DateTime? fromDate, DateTime? toDate) =>
{
    var disconnections = GetDisconnections(fromDate, toDate);
    return disconnections;
})
.WithName("GetDisconnections")
.WithOpenApi();

app.MapGet("/last-ping", () =>
{
    // read the lastSuccessfulConnection.txt file
    var lastConnection = File.ReadAllText(lastSuccessfulConnectionFile);
    return lastConnection;
})
.WithName("GetLastConnection")
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