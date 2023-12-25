using QuotebookApp.ViewModel;

namespace QuotebookApp.View;

public partial class QotdPage : ContentPage
{
	public QotdPage(QotdViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
}