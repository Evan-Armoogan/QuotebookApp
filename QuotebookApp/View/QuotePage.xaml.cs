namespace QuotebookApp.View;

public partial class QuotePage : ContentPage
{
	public QuotePage(QuoteViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
		viewModel.GetQuotesCommand.Execute(null);
	}
}