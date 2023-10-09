﻿using QuotebookApp.Services;
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
    int quoteeIndex;

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

    public bool IsNotAddQuote => !IsAddQuote;

    public ObservableCollection<string> Users { get; } = new ObservableCollection<string>();

    public ObservableCollection<Quote> Quotes { get; } = new ObservableCollection<Quote>();


    // private declarations
    private List<Quote> allQuotes;

    private void resetFilterOptions()
    {
        QuoteeIndex = -1;
        QuoterIndex = -1;
        QuoteDate = DateTime.Today;
    }


    public QuoteViewModel(QuoteService quoteService)
    {
        Title = "Quotebook";
        this.quoteService = quoteService;
        foreach (User user in GlobalData.Users)
        {
            Users.Add(user.UserName);
        }

        FilterBtnText = "Filter";
        IsAddQuote = false;

        // clear quotebook
        Quotes.Clear();
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

            if (QuoteeIndex == -1)
            {
                // Quotee was not sent, we should throw an exception
                throw new Exception("'Said By' parameter was not set.");
            }

            await quoteService.AddQuote(GlobalData.CurrentUser.UserName, Users[QuoteeIndex], NewQuoteString);

            await Shell.Current.DisplayAlert("Success", "Quote added to quotebook", "OK");

            Quote new_quote = new Quote(DateTime.Now, GlobalData.CurrentUser.UserName, Users[QuoteeIndex], NewQuoteString);
            new_quote.CreateTimestampString();
            new_quote.CreateQuoteeTimeString();
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
            QuoteeIndex = -1;
        }
    }

    [RelayCommand]
    void ExitAddQuote()
    {
        if (IsBusy)
            return;

        IsAddQuote = false;
        NewQuoteString = "";
        QuoteeIndex = -1;
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
        if (QuoteeIndex != -1)
            Quotee = GlobalData.Users[QuoteeIndex].UserName;
        else
            Quotee = "";

        if (QuoterIndex != -1)
            Quoter = GlobalData.Users[QuoterIndex].UserName;
        else
            Quoter = "";


        List<Quote> new_quotes = new List<Quote>();
        foreach (Quote quote in allQuotes)
        {
            if (Quotee != quote.Quotee && Quotee != "")
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

/*
 if (Quotee == "" && Quoter == "")
            {
                // only check timestamps
                if (AreDatesEqual(quote.Timestamp, QuoteDate))
                    new_quotes.Add(quote);
            }
            else if (Quotee == "" && Quoter != "")
            {
                // Check Quoter and timestamp
                if (quote.Quoter == Quoter && AreDatesEqual(quote.Timestamp, QuoteDate))
                    new_quotes.Add(quote);
            }
            else if (Quotee != "" && Quoter == "")
            {
                // Check quotee and timestamp
                if (quote.Quotee == Quotee && AreDatesEqual(quote.Timestamp, QuoteDate))
                    new_quotes.Add(quote);
            }
            else
            {
                // check all
                if (quote.Quotee == Quotee && quote.Quoter == Quoter && AreDatesEqual(quote.Timestamp, QuoteDate))
                    new_quotes.Add(quote);
            }
 */