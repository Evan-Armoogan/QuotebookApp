namespace QuotebookApp.View;

public partial class ChangelogPage : ContentPage
{
	public ChangelogPage(ChangelogViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
		viewModel.LoadChangelogMdCommand.Execute(null);
    }
}