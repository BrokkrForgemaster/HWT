namespace HWT.Domain.Entities;

/// <summary name="GameLogEvent">
/// This class represents a game log event sent to the server.
/// It contains the timestamp of the event, the type of event, and the payload.
/// The payload can be either raw or parsed details of the event.
/// </summary>
public class GameLogEvent
{
    public DateTime Timestamp { get; set; }
    public string EventType { get; set; } = string.Empty;
    public string? Payload { get; set; } = string.Empty;
    
    public string RawLine { get; set; } = string.Empty;
}