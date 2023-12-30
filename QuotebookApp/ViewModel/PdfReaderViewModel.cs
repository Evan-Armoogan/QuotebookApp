namespace QuotebookApp.ViewModel;

public partial class PdfReaderViewModel : BaseViewModel
{
    [ObservableProperty]
    WebViewSource pdfWebViewSource;

    private void ModifyWebView()
    {
        Microsoft.Maui.Handlers.WebViewHandler.Mapper.AppendToMapping("My Customization", (handler, view) =>
        {
#if ANDROID
            handler.PlatformView.Settings.JavaScriptEnabled = true;
            handler.PlatformView.Settings.AllowFileAccess = true;
            handler.PlatformView.Settings.AllowFileAccessFromFileURLs = true;
            handler.PlatformView.Settings.AllowUniversalAccessFromFileURLs = true;
#endif
        });
    }

    private void LoadPdfAndroid()
    {
        string pdfFilePath = $"file:///android_asset/pdfjs/web/viewer.html?file=file:///{GlobalData.TargetPdfFile}";
        PdfWebViewSource = new UrlWebViewSource { Url = pdfFilePath };
    }

    private void LoadPdfWindowsMac()
    {
        string encodedPath = Uri.EscapeDataString(GlobalData.TargetPdfFile);
        PdfWebViewSource = new UrlWebViewSource { Url = $"file:///{encodedPath}" };
    }

    public PdfReaderViewModel()
    {
        IsBusy = false;
        Title = "PDF Reader";

        ModifyWebView();

        try
        {
#if ANDROID
            LoadPdfAndroid();
#else
            LoadPdfWindowsMac();
            // No implementation at the moment for IOS. For now, will assume the Windows & Mac method works
#endif
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.ToString());
            Shell.Current.DisplayAlert("Error!", $"Error occurred while opening PDF file: {ex.Message}", "OK");
        }
    }
}
