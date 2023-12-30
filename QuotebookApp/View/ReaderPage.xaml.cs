namespace QuotebookApp.View;

public partial class ReaderPage : ContentPage
{
	public ReaderPage(ReaderViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
		viewModel.GetAvailableFilesCommand.Execute(null);
    }
}