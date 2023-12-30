using Google.Apis.Sheets.v4.Data;
using QuotebookApp.Services;

namespace QuotebookApp;

public class Initialization : GlobalData
{
    private const int client_height_offset = 140;

    public static void InitializePdfDirectory()
    {
        /* Ensure the PDF directory exists (doesn't first time using app) */
        string directory = GlobalData.PdfDownloadDirectory;
        if (!Directory.Exists(directory))
            Directory.CreateDirectory(directory);
    }

    public static void InitializeClientHeight()
    {
#if ANDROID || IOS
        ClientHeight = Convert.ToInt32(DeviceDisplay.Current.MainDisplayInfo.Height / DeviceDisplay.Current.MainDisplayInfo.Density - Shell.Current.Height);
#else
        ClientHeight = 790;
#endif
        ClientHeight -= client_height_offset;
    }

    public async static void InitializeApiParameters()
    {
        using var stream_url = await FileSystem.OpenAppPackageFileAsync("Secrets/SheetID.secret");
        using var reader_url = new StreamReader(stream_url);
        SheetID = await reader_url.ReadToEndAsync();
        reader_url.Close();
        stream_url?.Close();
    }


    public static void InitializeRetryStatusCodes()
    {
        List<int> retry_status_codes = new List<int>();
        retry_status_codes.Add(408);
        retry_status_codes.Add(429);
        for (int i = 500; i < 600; i++)
        {
            retry_status_codes.Add(i);
        }

        RetryStatusCodes = retry_status_codes;
    }

    public static void InitializeAppData()
    {
        InitializePdfDirectory();
        InitializeApiParameters();
        InitializeRetryStatusCodes();
        AppTheme.InitializeAppTheme();

        /* Initialize Google API services */
        BaseSheetService.InitializeClient();
        BaseDriveService.InitializeClient();
    }
}
