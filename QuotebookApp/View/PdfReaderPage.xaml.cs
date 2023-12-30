using System.Web;

namespace QuotebookApp.View;

public partial class PdfReaderPage : ContentPage
{
	public PdfReaderPage(PdfReaderViewModel viewModel)
	{
		InitializeComponent();
        BindingContext = viewModel;
	}
}