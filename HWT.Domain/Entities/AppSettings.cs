namespace HWT.Domain.Entities;

/// <summary name="AppSettings">
/// This class is used to bind the settings from the configuration file.
/// Represents user-configurable settings from appsettings.json,
/// which are used to configure the application.
/// The properties in this class correspond to the keys in the appsettings.json file.
/// </summary>
public class AppSettings
{
    public string? GameLogFilePath           { get; set; }
    public string? DiscordToken              { get; set; }
    public string? GoogleSheetsId            { get; set; }   
    public string? GoogleApiCredentialsPath  { get; set; }
    public string? StarCitizenApiKey         { get; set; }
    public string? StarCitizenApiUrl         { get; set; }
    public string? KillSheetKey              { get; set; }
    public string? Theme                     { get; set; }
    public string? DiscordWebhookUrl         { get; set; }
    public string? RsiToken                  { get; set; }
    public bool   ShowApiSettings            { get; set; } = false;
    public string? TradingApiUrl             { get; set; }  
    public string? TradingApiKey             { get; set; }   
}
