namespace HWT.Domain.Entities;

public abstract class RefineryJob
{
    public int Id { get; set; }
    public int IdTerminal { get; set; }
    public int IdRefineryMethod { get; set; }
    public decimal Cost { get; set; }         // decimal because cost may not be integer
    public int TimeMinutes { get; set; }
    public long DateAdded { get; set; }       // Unix timestamp
    public long DateModified { get; set; }    // Unix timestamp
    public long DateExpiration { get; set; }  // Unix timestamp
    public string TerminalName { get; set; }
    public List<RefineryItem> Items { get; set; }
}