using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;

namespace QuotebookApp.Services;

public class QuoteService
{
    BaseService service;

    public QuoteService()
    {
        service = new BaseService();
    }


    List<Quote> quoteList = new List<Quote>();

    private DateTime processQuoteTimestamp(string timestamp)
    {
        string[] timestamps = timestamp.Split(" ");
        string date = timestamps[0];
        string time = timestamps[1];

        // parse date
        string[] dates = date.Split("-");
        int year = Convert.ToInt32(dates[0]);
        int month = Convert.ToInt32(dates[1]);
        int day = Convert.ToInt32(dates[2]);

        // parse time
        string[] times = time.Split(":");
        int hour = Convert.ToInt32(times[0]);
        int minute = Convert.ToInt32(times[1]);
        int second = Convert.ToInt32(times[2]);

        // initialize DateTime object
        DateTime dt = new DateTime(year, month, day, hour, minute, second);
        return dt;
    }

    private Quote processNewQuote(string[] value)
    {
        /* parse timestamp string to create DT object */
        string timestamp = value[0];
        DateTime dt = processQuoteTimestamp(timestamp);

        Quote quote = new Quote(dt, value[1], value[2], value[3]);
        quote.CreateTimestampString();

        /* account for multi line quotes */
        if (quote.QuoteString.Contains(" // "))
        {
            quote.QuoteString = quote.QuoteString.Replace(" // ", Environment.NewLine.ToString());
        }

        /* account for multiple quotees */
        if (quote.Quotee.Contains(","))
        {
            int idx = quote.Quotee.LastIndexOf(", ");

            quote.Quotee = quote.Quotee.Remove(idx, ", ".Length).Insert(idx, " & ");
        }

        quote.CreateQuoteeTimeString();
        quote.CreateQuoterString();

        return quote;
    }

    public async Task<List<Quote>> GetQuotes()
    {
        quoteList.Clear();

        string range = "Quotes!A2:D";

        SheetData data = await service.GetResponse(range);

        foreach (string[] value in data.Values)
        {
            Quote quote = processNewQuote(value);
            quoteList.Add(quote);
        }

        return quoteList;
    }

    public async Task<Quote> GetQotd()
    {
        string range = "QOTD!A2:D2";
        SheetData data = await service.GetResponse(range);

        string[] result = data.Values[0];

        Quote quote = processNewQuote(result);
        return quote;
    }

    public async Task AddQuote(string quoter, string quotee, string quotestring)
    {
        DateTime timestamp = DateTime.Now;
        string year = timestamp.Year.ToString();
        string month = string.Format("{0:00}", timestamp.Month.ToString());
        string day = string.Format("{0:00}", timestamp.Day.ToString());
        string hour = string.Format("{0:00}", timestamp.Hour.ToString());
        string minute = string.Format("{0:00}", timestamp.Minute.ToString());
        string second = string.Format("{0:00}", timestamp.Second.ToString());

        string timestamp_string = $"{year}-{month}-{day} {hour}:{minute}:{second}";

        /* account for multi line quotes */
        if (quotestring.Contains("\r"))
        {
            quotestring = quotestring.Replace("\r", " // ");
        }
        else if (quotestring.Contains("\n"))
        {
            quotestring = quotestring.Replace("\n", " // ");
        }


        string range = "Quotes!A2:D";
        string[] values = new string[4];

        values[0] = timestamp_string;
        values[1] = quoter;
        values[2] = quotee;
        values[3] = quotestring;

        await service.SetResponse(range, values);
        return;
    }
}
