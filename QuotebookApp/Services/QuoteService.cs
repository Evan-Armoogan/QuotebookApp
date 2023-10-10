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

    public async Task<List<Quote>> GetQuotes()
    {
        quoteList.Clear();

        string range = "Quotes!A2:D";

        SheetData data = await service.GetResponse(range);

        foreach (string[] value in data.Values)
        {
            // parse timestamp string to create DT object
            string timestamp = value[0];
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

            Quote quote = new Quote(dt, value[1], value[2], value[3]);
            quote.CreateTimestampString();

            // multi-line quotes
            if (quote.QuoteString.Contains(" // "))
            {
                quote.QuoteString = quote.QuoteString.Replace(" // ", "\n");
            }
            if (quote.Quotee.Contains(","))
            {
                int idx = quote.Quotee.LastIndexOf(", ");

                quote.Quotee = quote.Quotee.Remove(idx, ", ".Length).Insert(idx, " & ");
            }

            quote.CreateQuoteeTimeString();

            quoteList.Add(quote);
        }

        return quoteList;
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
