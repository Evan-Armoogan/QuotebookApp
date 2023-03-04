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
        if (quoteList?.Count > 0)
            return quoteList;

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

            quoteList.Add(quote);
        }

        return quoteList;
    }
}
