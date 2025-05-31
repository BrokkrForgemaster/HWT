using HWT.Domain.Entities;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Sheets.v4.Data;
using HWT.Application.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace HWT.Infrastructure.Services;

/// <summary>
/// This service handles interactions with Google Sheets for logging kills.
/// It uses the Google Sheets API to append new kill entries to a specified spreadsheet,
/// specifically in the "Raw Log" sheet.
/// </summary>
public class GoogleSheetService : IGoogleSheetService
{
    #region Fields
    private readonly SheetsService _sheets;
    private readonly string        _spreadsheetId;
    private readonly string        _rawLogRange = "Raw Log!A:E";
    private readonly ILogger<GoogleSheetService> _logger;
    #endregion
    
    #region Constructor
    public GoogleSheetService(IOptions<AppSettings> opts, ILogger<GoogleSheetService> logger)
    {
        _logger = logger;
        var settings = opts.Value;
        
        var credPath = settings.GoogleApiCredentialsPath;

        if (string.IsNullOrWhiteSpace(credPath) || !File.Exists(credPath))
        {
            _logger.LogWarning("Google Sheets credentials not found. Logging is disabled.");
            _sheets = null;
            return; 
        }
        
        var creds = GoogleCredential.FromFile(credPath)
            .CreateScoped(SheetsService.Scope.Spreadsheets);

        _sheets = new SheetsService(new BaseClientService.Initializer
        {
            HttpClientInitializer = creds,
            ApplicationName = "Kill Tracker",
        });

        _spreadsheetId     = settings.GoogleSheetsId 
                             ?? throw new ArgumentException("GoogleSheetsId not configured");
        
    }
    #endregion
    
    #region Methods
    /// <summary name="LogKillAsync">
    /// Logs a kill entry to the Google Sheet in the "Raw Log" sheet.
    /// This method appends a new row with the kill details such as timestamp, attacker,
    /// type, target, and weapon.
    /// <param name="kill">
    /// The kill entry to log, containing details like timestamp, attacker,
    /// type, target, and weapon.</param>
    /// <returns>
    /// A task that represents the asynchronous operation of logging the kill entry.</returns>
    /// <exception cref="Google.GoogleApiException">
    /// Thrown when there is an error with the Google Sheets API, such as permission issues,
    /// invalid spreadsheet ID, or network errors.</exception>
    /// <exception cref="Exception"> Thrown when there is an unexpected error during the logging process,
    /// such as serialization issues or other runtime exceptions.</exception>
    /// </summary>
    public async Task LogKillAsync(KillEntry kill)
    {
        // Skip logging if this is an "unknown" kill
        if (string.Equals(kill.Type, "Unknown", StringComparison.OrdinalIgnoreCase)
            || string.Equals(kill.Attacker, "Unknown", StringComparison.OrdinalIgnoreCase)
            || string.Equals(kill.Target, "Unknown", StringComparison.OrdinalIgnoreCase))
        {
            _logger.LogWarning("Skipping unknown kill: {Summary}", kill.Summary);
            return;
        }

        _logger.LogInformation("Logging kill to Google Sheet: {Summary}", kill.Summary);

        var row = new List<object>
        {
            kill.Timestamp,
            kill.Attacker,
            kill.Type,
            kill.Target,
            kill.Weapon
        };

        var valueRange = new ValueRange { Values = [row] };
        var appendRequest = _sheets.Spreadsheets.Values
            .Append(valueRange, _spreadsheetId, _rawLogRange);
        appendRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.AppendRequest.ValueInputOptionEnum.USERENTERED;

        try
        {
            var response = await appendRequest.ExecuteAsync();
            var updatedCells = response.Updates?.UpdatedCells ?? 0;
            _logger.LogInformation(
                "Kill logged successfully: {Summary}. Cells updated: {Count}",
                kill.Summary, updatedCells);
        }
        catch (Google.GoogleApiException gex)
        {
            _logger.LogError(
                gex,
                "Google API error while logging kill: {Summary}. Status: {Status}, Message: {Message}",
                kill.Summary, gex.Error?.Code, gex.Error?.Message);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Unexpected error while logging kill to Google Sheet: {Summary}",
                kill.Summary);
            throw;
        }
    }   
    #endregion
}