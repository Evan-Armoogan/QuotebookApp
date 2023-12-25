using QuotebookApp.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuotebookApp.ViewModel;

public partial class QuoteViewModel : BaseViewModel
{
    QuoteService quoteService;

    [ObservableProperty]
    bool filterQuotes;

    [ObservableProperty]
    string filterBtnText;

    [ObservableProperty]
    int quotee1Index;

    [ObservableProperty]
    int quotee2Index;

    [ObservableProperty]
    int quoterIndex;

    [ObservableProperty]
    DateTime quoteDate;

    [ObservableProperty]
    bool filterDate;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsNotAddQuote))]
    bool isAddQuote;

    [ObservableProperty]
    string newQuoteString;

    [ObservableProperty]
    int quoteListHeight;

    [ObservableProperty]
    int topButtonHeight;

    [ObservableProperty]
    int filterFrameHeight;

    [ObservableProperty]
    int filterFrameSpacing;

    [ObservableProperty]
    LayoutOptions quoteListAlignment;

    public bool IsNotAddQuote => !IsAddQuote;

    public ObservableCollection<string> Users { get; } = new ObservableCollection<string>();

    public ObservableCollection<Quote> Quotes { get; } = new ObservableCollection<Quote>();


    // private declarations
    private List<Quote> allQuotes;

    private void resetFilterOptions()
    {
        Quotee1Index = -1;
        Quotee2Index = -1;
        QuoterIndex = -1;
        QuoteDate = DateTime.Today;
    }

    private void setQuoteListProperties()
    {
        TopButtonHeight = 45;
        // this sucks to do, but it's the only way since there is a bug with .NET MAUI CollectionView scrolling
#if ANDROID || IOS
        QuoteListHeight = GlobalData.ClientHeight - (TopButtonHeight + 15);
        QuoteListAlignment = LayoutOptions.Center;
        FilterFrameSpacing = 0;
        FilterFrameHeight = 55;
#else
        QuoteListHeight = GlobalData.ClientHeight - (TopButtonHeight + 15);
        //QuoteListHeight = 590;
        QuoteListAlignment = LayoutOptions.Fill;
        FilterFrameSpacing = 20;
        FilterFrameHeight = 70;
#endif
    }


    public QuoteViewModel(QuoteService quoteService)
    {
        Title = "Quotebook";
        IsBusy = false;
        this.quoteService = quoteService;
        foreach (User user in GlobalData.Users)
        {
            Users.Add(user.UserName);
        }

        FilterBtnText = "Filter";
        IsAddQuote = false;

        // clear quotebook
        Quotes.Clear();

        setQuoteListProperties();
    }

    [RelayCommand]
    async Task GetQuotesAsync()
    {
        if (IsBusy)
            return;

        resetFilterOptions();

        try
        {
            IsBusy = true;
            List<Quote> quotes = await quoteService.GetQuotes();

            if (Quotes.Count != 0)
                Quotes.Clear();
            
            foreach (Quote quote in quotes)
            {
                Quotes.Add(quote);
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.ToString());
            await Shell.Current.DisplayAlert("Error!", $"Unable to retrieve list of quotes: {ex.Message}", "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    async Task AddQuoteAsync()
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;

            if (Quotee1Index == -1)
            {
                // Quotee was not sent, we should throw an exception
                throw new Exception("'Said By' parameter was not set.");
            }

            string quotee = Quotee2Index == -1 ? Users[Quotee1Index] : $"{Users[Quotee1Index]}, {Users[Quotee2Index]}";
            await quoteService.AddQuote(GlobalData.CurrentUser.UserName, quotee, NewQuoteString);

            await Shell.Current.DisplayAlert("Success", "Quote added to quotebook", "OK");

            string quotee_display = Quotee2Index == -1 ? Users[Quotee1Index] : $"{Users[Quotee1Index]} & {Users[Quotee2Index]}";
            Quote new_quote = new Quote(DateTime.Now, GlobalData.CurrentUser.UserName, quotee_display, NewQuoteString);
            new_quote.CreateTimestampString();
            new_quote.CreateQuoteeTimeString();
            new_quote.CreateQuoterString();
            Quotes.Add(new_quote);
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.ToString());
            await Shell.Current.DisplayAlert("Error!", $"Unable to upload quote to quotebook: {ex.Message}", "OK");
        }
        finally
        {
            IsBusy = false;
            IsAddQuote = false;
            NewQuoteString = "";
            Quotee1Index = -1;
            Quotee2Index = -1;
        }
    }

    [RelayCommand]
    void ExitAddQuote()
    {
        if (IsBusy)
            return;

        IsAddQuote = false;
        NewQuoteString = "";
        Quotee1Index = -1;
        Quotee2Index = -1;
    }

    [RelayCommand]
    void CreateQuote()
    {
        if (IsBusy)
            return;

        IsAddQuote = true;
    }

    [RelayCommand]
    void FilterOptions()
    {
        if (IsBusy)
            return;

        if (!FilterQuotes)
        {
            FilterQuotes = true;
            FilterBtnText = "Close Filter";

            // initialize quote filter pickers
            resetFilterOptions();

            // set new height for quote list
            QuoteListHeight -= FilterFrameHeight;

            // copy all our existing quotes to memory
            allQuotes = new List<Quote>();
            foreach (Quote quote in Quotes)
            {
                allQuotes.Add(quote);
            }
        }
        else
        {
            FilterQuotes = false;
            FilterBtnText = "Filter";

            // set new height for quote list
            QuoteListHeight += FilterFrameHeight;

            // restore all quotes
            Quotes.Clear();
            foreach (Quote quote in allQuotes)
            {
                Quotes.Add(quote);
            }
        }
    }

    private bool AreDatesEqual(DateTime dt1, DateTime dt2)
    {
        if ((dt1.Year == dt2.Year) && (dt1.Month == dt2.Month) && (dt1.Day == dt2.Day))
            return true;
        return false;
    }

    [RelayCommand]
    void FilterQuotesList()
    {
        if (IsBusy)
            return;

        string Quotee;
        string Quoter;
        if (Quotee1Index != -1)
            Quotee = GlobalData.Users[Quotee1Index].UserName;
        else
            Quotee = "";

        if (QuoterIndex != -1)
            Quoter = GlobalData.Users[QuoterIndex].UserName;
        else
            Quoter = "";


        List<Quote> new_quotes = new List<Quote>();
        foreach (Quote quote in allQuotes)
        {
            /* use contains to account for multi-person quotes */
            if (!quote.Quotee.Contains(Quotee) && Quotee != "")
                continue;
            if (Quoter != quote.Quoter && Quoter != "")
                continue;
            if (FilterDate && !AreDatesEqual(quote.Timestamp, QuoteDate))
                continue;

            new_quotes.Add(quote);
        }

        Quotes.Clear();
        foreach (Quote quote in new_quotes)
        {
            Quotes.Add(quote);
        }
    }
}