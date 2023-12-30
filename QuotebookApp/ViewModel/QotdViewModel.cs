using QuotebookApp.Services;

namespace QuotebookApp.ViewModel;

public partial class QotdViewModel : BaseViewModel
{
    QuoteService quoteService;

    List<Quote> quotes;
    List<int> prevChosenQuotes;

    [ObservableProperty]
    string qotdQuoteString;

    [ObservableProperty]
    string qotdQuoteeTimeString;

    [ObservableProperty]
    string qotdQuoterString;

    [ObservableProperty]
    string randomQuoteString;

    [ObservableProperty]
    string randomQuoteeTimeString;

    [ObservableProperty]
    string randomQuoterString;

    [ObservableProperty]
    int clientHeight;


    public QotdViewModel(QuoteService quoteService)
    {
        this.quoteService = quoteService;
        IsBusy = false;
        Title = "Quote of the Day";
        ClientHeight = GlobalData.ClientHeight;
    }

    [RelayCommand]
    public async Task GetQotdAsync()
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;

            Quote quote = await quoteService.GetQotd();

            QotdQuoteString = quote.QuoteString;
            QotdQuoteeTimeString = quote.QuoteeTimeString;
            QotdQuoterString = quote.QuoterString;
        }
        catch(Exception ex)
        {
            Debug.WriteLine(ex.ToString());
            await Shell.Current.DisplayAlert("Error!", $"Unable to retrieve Quote of the Day: {ex.Message}", "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    public async Task GetRandomQuoteAsync()
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;

            if (quotes is null)
            {
                quotes = await quoteService.GetQuotes();
            }
            if (prevChosenQuotes is null)
            {
                prevChosenQuotes = new List<int>();
            }

            if (prevChosenQuotes.Count == quotes.Count)
            {
                /* remove first half of quotes and recycle them */
                int remove = Convert.ToInt32(quotes.Count * 0.5);
                for (int i = 0; i < remove; i++)
                {
                    prevChosenQuotes.RemoveAt(0);
                }
            }

            /* choose a random quote */
            int max = quotes.Count - 1;
            int num;
            do
            {
                Random rnd = new Random();
                num = rnd.Next(0, max);
            } while (prevChosenQuotes.Contains(num));
            prevChosenQuotes.Add(num);

            Quote quote = quotes[num];

            RandomQuoteString = quote.QuoteString;
            RandomQuoteeTimeString = quote.QuoteeTimeString;
            RandomQuoterString = quote.QuoterString;
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.ToString());
            await Shell.Current.DisplayAlert("Error!", $"Unable to retrieve Random Quote: {ex.Message}", "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    public async Task InitializeQotdPageAsync()
    {
        await GetQotdAsync();
        await GetRandomQuoteAsync();
    }
}
