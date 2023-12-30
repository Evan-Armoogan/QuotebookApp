namespace QuotebookApp.Model;

public class Quote
{
    public Quote(DateTime timestamp, string quoter, string quotee, string quote)
    {
        this.Timestamp = timestamp;
        this.Quoter = quoter;
        this.Quotee = quotee;
        this.QuoteString = quote;
    }

    public DateTime Timestamp { get; set; }
    public string Quoter { get; set; }
    public string Quotee { get; set; }
    public string QuoteString { get; set; }
    public string TimestampString { get; set; }
    public string QuoteeTimeString { get; set; }
    public string QuoterString { get; set; }

    public void CreateTimestampString()
    {
        TimestampString = Timestamp.ToLongDateString();
    }
    public void CreateQuoteeTimeString()
    {
        QuoteeTimeString = "-" + Quotee + ", " + TimestampString;
    }
    public void CreateQuoterString()
    {
        QuoterString = "Added by: " + Quoter;
    }
}
