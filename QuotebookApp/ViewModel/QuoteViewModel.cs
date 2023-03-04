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

    public ObservableCollection<Quote> Quotes { get; } = new ObservableCollection<Quote>();

    public QuoteViewModel(QuoteService quoteService)
    {
        Title = "Quotebook";
        this.quoteService = quoteService;
    }

    [RelayCommand]
    async Task GetQuotesAsync()
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;
            List<Quote> quotes = await quoteService.GetQuotes();

            if (Quotes.Count != 0)
                Quotes.Clear();
            
            foreach (Quote quote in quotes)
                Quotes.Add(quote);
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
}
