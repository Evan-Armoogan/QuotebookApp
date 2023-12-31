using Markdig;

namespace QuotebookApp.ViewModel;

public partial class ChangelogViewModel : BaseViewModel
{
    [ObservableProperty]
    HtmlWebViewSource changelogHtmlSource;

    public ChangelogViewModel()
    {
        IsBusy = false;
        Title = "Changelog";
    }

    [RelayCommand]
    async Task LoadChangelogMdAsync()
    {
        Stream mdStream = await FileSystem.OpenAppPackageFileAsync("changelog.md");
        StreamReader mdReader = new StreamReader(mdStream);
        string mdFile = mdReader.ReadToEnd();
        mdStream.Close();
        mdReader.Close();

        string result = Markdown.ToHtml(mdFile);

        HtmlWebViewSource changelogSource = new HtmlWebViewSource { Html = result };
        ChangelogHtmlSource = changelogSource;
    }
}
