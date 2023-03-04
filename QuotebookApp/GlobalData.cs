using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuotebookApp;

public class GlobalData
{

    public static string BaseURL { get; } = "https://sheets.googleapis.com/v4/spreadsheets";
    public static string APIKey { get; private set; }
    public static string SheetID { get; private set; }
    public static List<int> RetryStatusCodes { get; private set; }
    public static User CurrentUser { get; set; }


    public static bool IsLoggedIn { get; set; } = false;

    public static void ResetFlyoutMenu()
    {
        
    }


    public async static void InitializeApiParameters()
    {
        using var stream_key = await FileSystem.OpenAppPackageFileAsync("Secrets/APIKey.secret");
        using var reader_key = new StreamReader(stream_key);
        APIKey = await reader_key.ReadToEndAsync();
        reader_key.Close();
        stream_key?.Close();

        using var stream_url = await FileSystem.OpenAppPackageFileAsync("Secrets/SheetID.secret");
        using var reader_url = new StreamReader(stream_url);
        SheetID = await reader_url.ReadToEndAsync();
        reader_url.Close();
        stream_url?.Close();
    }

    public static void IniitalizeRetryStatusCodes()
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


}
