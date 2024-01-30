using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;

var disconnectionsPath = Environment.GetEnvironmentVariable("FAILURES_DATA") ?? "data/connectionFailures.csv";

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add CORS services
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowLocalhostOrigins", builder =>
    {
        builder.WithOrigins("http://localhost:5173")
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

var port = Environment.GetEnvironmentVariable("PORT") ?? "5000";

builder.WebHost.UseUrls($"http://*:{port}");

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Use CORS middleware
app.UseCors("AllowLocalhostOrigins");

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
    var lastConnection = File.ReadAllText("data/lastSuccessfulConnection.txt");
    return lastConnection;
})
.WithName("GetLastConnection")
.WithOpenApi();

app.Run();

List<Disconnection> GetDisconnections(DateTime? fromDate, DateTime? toDate)
{
    // read data from connectionFailures.csv
    using var reader = new StreamReader(disconnectionsPath);
    var config = new CsvConfiguration(CultureInfo.InvariantCulture)
    {
        HasHeaderRecord = false,
        Delimiter = ","
    };
    using var csv = new CsvReader(reader, config);
    var disconnections = csv.GetRecords<Disconnection>().ToList();

    // filter by date
    if(fromDate.HasValue)
    {
        disconnections = disconnections.Where(d => d.Start >= fromDate).ToList();
    }
    if(toDate.HasValue)
    {
        disconnections = disconnections.Where(d => d.Start <= toDate).ToList();
    }

    return disconnections;
}