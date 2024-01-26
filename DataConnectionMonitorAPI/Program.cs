using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.Configuration.Attributes;
using System.Globalization;


const string disconnectionsPath = "connectionFailures.csv";

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/disconnections", () =>
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
    return disconnections;
})
.WithName("GetDisconnections")
.WithOpenApi();

app.Run();

class Disconnection
{
    [Index(0)]
    public DateTime Start { get; set; }
    [Index(1)]
    public double Duration { get; set; }

    public string Downtime {
        get
        {
            var downtime = TimeSpan.FromSeconds(Duration);
            return downtime.ToString(@"hh\:mm\:ss");
        }
    }
}
