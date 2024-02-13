using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace DataConnectionMonitorAPI
{
  [EnableCors("AllowAllOrigins")]
  [ApiController]
  [Route("api/[controller]")]
  public class DisconnectionsController : ControllerBase
  {
    private readonly ILogger<DisconnectionsController> _logger;
    private readonly Config _configuration;
    private readonly string _disconnectionsFile;

    public DisconnectionsController(ILogger<DisconnectionsController> logger, IOptions<Config> configuration)
    {
      _logger = logger;
      _configuration = configuration.Value;
      _disconnectionsFile = _configuration.DisconnectionsFile ?? "";

      if (string.IsNullOrEmpty(_disconnectionsFile))
      {
        _logger.LogError("DisconnectionsFile is not set");
        throw new InvalidOperationException("DisconnectionsFile is not set");
      }
    }

    [HttpGet]
    public IEnumerable<Disconnection> Get(DateTime? fromDate, DateTime? toDate)
    {
      using var reader = new StreamReader(_disconnectionsFile);
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
  }
}