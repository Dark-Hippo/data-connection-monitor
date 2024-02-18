namespace DataConnectionMonitorAPI
{
  public class Config
  {
    public string? DisconnectionsFile { get; set; }
    public string? LastSuccessfulConnectionFile { get; set; }
    public string? CurrentStatusFile { get; set; }
    public int Port { get; set; }
  }
}