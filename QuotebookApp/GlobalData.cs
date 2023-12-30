namespace QuotebookApp;

public class GlobalData
{

    public static string BaseURL { get; } = "https://sheets.googleapis.com/v4/spreadsheets";
    public static string SheetID { get; protected set; }
    public static List<int> RetryStatusCodes { get; protected set; }
    public static User CurrentUser { get; set; }
    public static List<User> Users { get; set; }
    public static int ClientHeight { get; protected set; }

    public static string PdfDownloadDirectory { get; } = Path.Combine(FileSystem.Current.AppDataDirectory, "QuotebookAppPdfDownloads");
    public static string TargetPdfFile { get; set; }


    public static bool IsLoggedIn { get; set; } = false;
}
