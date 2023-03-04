using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
}
