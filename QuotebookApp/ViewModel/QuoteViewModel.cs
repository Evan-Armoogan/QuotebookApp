namespace QuotebookApp.ViewModel;

public partial class QuoteViewModel : BaseViewModel
{
    QuoteService quoteService;
    Quote selectedQuote;
    Predicate<Quote> quoteSearchPredicate;

    [ObservableProperty]
    bool filterQuotes;

    [ObservableProperty]
    string filterBtnText;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Quotee1IsOther))]
    int quotee1Index;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Quotee2IsOther))]
    int quotee2Index;

    [ObservableProperty]
    string quotee1Other;

    [ObservableProperty]
    string quotee2Other;

    [ObservableProperty]
    int quoterIndex;

    [ObservableProperty]
    DateTime quoteDate;

    [ObservableProperty]
    int filterQuoteeIndex;

    [ObservableProperty]
    int filterQuoterIndex;

    [ObservableProperty]
    bool filterDate;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsNotChangeQuote))]
    bool isChangeQuote;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(AddQuoteButtonsColumnDefinitions))]
    [NotifyPropertyChangedFor(nameof(AddQuoteButtonText))]
    bool isEditDeleteQuote; // this is used to indicate if the delete button is shown

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

    public bool IsNotChangeQuote => !IsChangeQuote;

    public bool Quotee1IsOther => Quotee1Index == Users.Count() - 1;
    public bool Quotee2IsOther => Quotee2Index == Users.Count() - 1;

    public string AddQuoteButtonText => IsEditDeleteQuote ? "Update" : "Add";
    public ColumnDefinitionCollection AddQuoteButtonsColumnDefinitions => IsEditDeleteQuote ?
        (new ColumnDefinitionCollection { new ColumnDefinition(), new ColumnDefinition(), new ColumnDefinition() }) :
        (new ColumnDefinitionCollection { new ColumnDefinition(), new ColumnDefinition() });

    public ObservableCollection<string> Users { get; } = new ObservableCollection<string>();

    public ObservableCollection<Quote> Quotes { get; } = new ObservableCollection<Quote>();

    // private declarations
    private List<Quote> allQuotes;

    private void resetFilterOptions()
    {
        FilterQuoteeIndex = -1;
        FilterQuoterIndex = -1;
        QuoteDate = DateTime.Today;
    }

    private void setCollectionListProperties()
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
        QuoteListAlignment = LayoutOptions.Fill;
        FilterFrameSpacing = 20;
        FilterFrameHeight = 70;
#endif
    }

    private void initializeQuoteSearchPredicate()
    {
        quoteSearchPredicate = new Predicate<Quote>(q => (q.Quotee == selectedQuote.Quotee &&
                                                          q.Quoter == selectedQuote.Quoter &&
                                                          q.QuoteString == selectedQuote.QuoteString &&
                                                          AreDatesEqual(q.Timestamp, selectedQuote.Timestamp)));
    }


    public QuoteViewModel(QuoteService quoteService)
    {
        Title = "Quotebook";
        IsBusy = false;
        this.quoteService = quoteService;
        foreach (User user in GlobalData.Users)
        {
            /* Don't add invisible users to pickers, point of an invisible
             * user is that other app users do not see them, but they have access */
            if (user.UserType != User.UserPermissionType.Invisible)
                Users.Add(user.UserName);
        }
        Users.Add("Other");

        FilterBtnText = "Filter";
        IsChangeQuote = false;

        Quotee1Index = -1;
        Quotee2Index = -1;

        // clear quotebook
        Quotes.Clear();

        initializeQuoteSearchPredicate();
        setCollectionListProperties();
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

            /* Reverse the list of quotes so most recent quotes appear first */
            quotes.Reverse();

            if (Quotes.Count != 0)
                Quotes.Clear();

            allQuotes = new List<Quote>();
            foreach (Quote quote in quotes)
            {
                Quotes.Add(quote);
                allQuotes.Add(quote);
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

    private string ProcessQuoteeStringForUpload()
    {
        string quotee1 = "";
        if (Quotee1IsOther)
            quotee1 = Quotee1Other;
        else
            quotee1 = Users[Quotee1Index];

        string quotee2 = "";
        if (Quotee2Index != -1) // Only process quotee2 if one has been set
        {
            if (Quotee2IsOther)
                quotee2 = Quotee2Other;
            else
                quotee2 = Users[Quotee2Index];
        }

        string quotee = Quotee2Index == -1 ? quotee1 : $"{quotee1}, {quotee2}";

        return quotee;
    }

    private Quote ProcessQuoteStringForDisplay(DateTime timestamp, string quoter, string quotee, string quotestring)
    {
        string quotee_display = quotee.Replace(", ", " & ");
        Quote new_quote = new Quote(timestamp, quoter, quotee_display, quotestring);
        new_quote.CreateTimestampString();
        new_quote.CreateQuoteeTimeString();
        new_quote.CreateQuoterString();
        return new_quote;
    }

    private void exitAndResetAddQuotePage()
    {
        IsChangeQuote = false;
        NewQuoteString = "";
        Quotee1Index = -1;
        Quotee2Index = -1;
        Quotee1Other = "";
        Quotee2Other = "";
    }

    private void checkQuoteeValidity()
    {
        if (Quotee1Index == -1)
        {
            /* Quotee was not sent, we should throw an exception */
            throw new Exception("'Said By' parameter was not set.");
        }

        if ((Quotee1IsOther && (Quotee1Other == "" || Quotee1Other is null)) || 
            (Quotee2IsOther && (Quotee2Other == "" || Quotee2Other is null)))
        {
            /* One of the quotees is blank (and can't be) */
            throw new Exception("'Said By' and 'And By' parameters may not be left blank");
        }
    }

    private async Task addQuoteAsync()
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;

            checkQuoteeValidity();

            string quotee = ProcessQuoteeStringForUpload();

            await quoteService.AddQuote(GlobalData.CurrentUser.UserName, quotee, NewQuoteString);

            Quote new_quote = ProcessQuoteStringForDisplay(DateTime.Now, GlobalData.CurrentUser.UserName, quotee, NewQuoteString);
            allQuotes.Insert(0, new_quote);

            await Shell.Current.DisplayAlert("Success", "Quote added to quotebook", "OK");
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.ToString());
            await Shell.Current.DisplayAlert("Error!", $"Unable to upload quote to quotebook: {ex.Message}", "OK");
        }
        finally
        {
            IsBusy = false;
            /* Need to refilter the quotes in case the added quote matches the filter description */
            FilterQuotesList();
            exitAndResetAddQuotePage();
        }
    }

    private async Task editQuoteAsync()
    {
        if (IsBusy)
            return;

        bool answer = await Shell.Current.DisplayAlert("Confirmation", "Are you sure you want to edit this quote?", "Yes", "No");
        if (!answer)
            return;

        try
        {
            IsBusy = true;

            checkQuoteeValidity();

            string quotee = ProcessQuoteeStringForUpload();

            /* Define two quotes as equal when the quoter, quotee, quote are the same and the dates are equal */
            int all_quote_idx = allQuotes.FindIndex(quoteSearchPredicate);
            int quote_idx = Quotes.ToList().FindIndex(quoteSearchPredicate);
            int sheet_idx = allQuotes.Count() - all_quote_idx - 1; // because allQuotes is in reverse order of spreadsheet

            await quoteService.EditQuote(selectedQuote.Quoter, quotee, NewQuoteString, selectedQuote.Timestamp, sheet_idx);

            Quote new_quote = ProcessQuoteStringForDisplay(selectedQuote.Timestamp, selectedQuote.Quoter, quotee, NewQuoteString);
            allQuotes[all_quote_idx] = new_quote;
            Quotes[quote_idx] = new_quote;

            await Shell.Current.DisplayAlert("Success", "Quote edited successfully", "OK");
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.ToString());
            await Shell.Current.DisplayAlert("Error!", $"Unable to edit the requested quote: {ex.Message}", "OK");
        }
        finally
        {
            IsBusy = false;
            exitAndResetAddQuotePage();
        }
    }

    [RelayCommand]
    async Task UpdateQuoteAsync()
    {
        if (!IsEditDeleteQuote)
            await addQuoteAsync();
        else
            await editQuoteAsync();
    }

    [RelayCommand]
    async Task DeleteQuoteAsync()
    {
        if (IsBusy)
            return;

        bool answer = await Shell.Current.DisplayAlert("Confirmation", "Are you sure you want to delete this quote?", "Yes", "No");
        if (!answer)
            return;

        try
        {
            IsBusy = true;

            int quote_idx = allQuotes.FindIndex(quoteSearchPredicate);
            int sheet_idx = allQuotes.Count() - quote_idx - 1; // because allQuotes is in reverse order of spreadsheet
            await quoteService.DeleteQuote(sheet_idx);

            Quote remove_quote = allQuotes[quote_idx];
            allQuotes.Remove(remove_quote);

            await Shell.Current.DisplayAlert("Success!", "Quote successfully deleted.", "OK");
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.ToString());
            await Shell.Current.DisplayAlert("Error!", $"Unable to delete the requested quote: {ex.Message}", "OK");
        }
        finally
        {
            IsBusy = false;
            /* Need to refilter the quotes in case the deleted quote matched the filter description */
            FilterQuotesList();
            exitAndResetAddQuotePage();
        }
    }

    [RelayCommand]
    void ExitAddQuote()
    {
        if (IsBusy)
            return;

        exitAndResetAddQuotePage();
    }

    [RelayCommand]
    void CreateQuote()
    {
        if (IsBusy)
            return;

        if (GlobalData.CurrentUser.UserType == User.UserPermissionType.Invisible)
        {
            Shell.Current.DisplayAlert("Warning!", "Users with your permission level are not allowed to add quotes. Contact your app administrator.", "OK");
            return;
        }

        IsEditDeleteQuote = false;
        IsChangeQuote = true;
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
        if (FilterQuoteeIndex != -1)
            Quotee = GlobalData.Users[FilterQuoteeIndex].UserName;
        else
            Quotee = "";

        if (FilterQuoterIndex != -1)
            Quoter = GlobalData.Users[FilterQuoterIndex].UserName;
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

    [RelayCommand]
    void QuoteTapped(Quote quote)
    {
        /* Invisible users are not allowed to edit quotes */
        if (GlobalData.CurrentUser.UserType == User.UserPermissionType.Invisible)
        {
            Shell.Current.DisplayAlert("Warning!", "Users with your permission level are not allowed to edit or delete quotes. Contact your app administrator.", "OK");
            return;
        }

        /* Check if the user is allowed to edit/delete the quote */
        string[] quotees = quote.Quotee.Split(" & ");
        string quoter = quote.Quoter;
        string username = GlobalData.CurrentUser.UserName;

        /* Only allowed to edit quotes if you are quoted in them or if you quoted them */
        bool canEditQuote = false;
        foreach (string quotee in quotees)
        {
            if (quotee == username)
                canEditQuote = true;
        }
        if (quoter == username)
            canEditQuote = true;

        /* Admins can edit/delete all quotes */
        if (GlobalData.CurrentUser.UserType == User.UserPermissionType.Admin)
            canEditQuote = true;

        if (!canEditQuote)
        {
            Shell.Current.DisplayAlert("Warning!", "You may only edit and delete quotes that you quoted or are quoted in.", "OK");
            return;
        }

        /* User is allowed to edit quote, start processing it */
        selectedQuote = quote;

        /* Fill in quote, said by boxes with the quote info */
        NewQuoteString = quote.QuoteString;

        List<int> quoteeIndex = new List<int>();
        foreach (string quotee in quotees)
        {
            int idx = 0;
            foreach (string user in Users)
            {
                /* Add the index if the quotee is a valid user, or if quotee is "Other"  */
                if (user == quotee || idx == Users.Count() - 1)
                {
                    quoteeIndex.Add(idx);
                    break;
                }
                idx++;
            }
        }

        Quotee1Index = -1;
        Quotee2Index = -1;
        if (quoteeIndex.Count > 0)
            Quotee1Index = quoteeIndex[0];
        if (quoteeIndex.Count > 1)
            Quotee2Index = quoteeIndex[1];

        if (Quotee1IsOther)
            Quotee1Other = quotees[0];
        if (Quotee2IsOther)
            Quotee2Other = quotees[1];

        /* Set up the edit/delete/add quote controls and make them visible */
        IsEditDeleteQuote = true;
        IsChangeQuote = true;
    }
}
