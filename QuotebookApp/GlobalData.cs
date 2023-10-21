namespace QuotebookApp;

public class GlobalData
{

    public static string BaseURL { get; } = "https://sheets.googleapis.com/v4/spreadsheets";
    public static string SheetID { get; private set; }
    public static List<int> RetryStatusCodes { get; private set; }
    public static User CurrentUser { get; set; }
    public static List<User> Users { get; set; }
    public static string AppTheme { get; set; }


    public static bool IsLoggedIn { get; set; } = false;



    public async static void InitializeApiParameters()
    {
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

    public static void InitializeAppTheme()
    {
        string[] themes = { "Light", "Dark", "Device Default" };
        string directory = $"{FileSystem.AppDataDirectory}/";
        string target = Path.Combine(directory, "AppTheme.txt");

        if (File.Exists(target))
        {
            FileStream stream = File.OpenRead(target);
            StreamReader reader = new StreamReader(stream);
            string result = reader.ReadToEnd();
            AppTheme = result;

            reader?.Close();
            stream?.Close();

            if (!themes.Contains(result))
            {
                /* Bad data, empty file and rewrite default */
                FileStream streamReset = File.OpenWrite(target);
                streamReset.SetLength(0);
                streamReset.Close(); // ensures file is wiped
                FileStream streamWrite = File.OpenWrite(target);
                StreamWriter writerR = new StreamWriter(streamWrite);
                writerR.Write(themes[2]);
                writerR?.Close();
                streamWrite?.Close();
                AppTheme = themes[2];
            }
        }
        else
        {
            using FileStream stream = File.OpenWrite(target);
            using StreamWriter writer = new StreamWriter(stream);
            writer.Write(themes[2]);
            AppTheme = themes[2];
            writer?.Close();
            stream?.Close();
        }

        if (AppTheme == "Light")
            Application.Current.UserAppTheme = Microsoft.Maui.ApplicationModel.AppTheme.Light;
        else if (AppTheme == "Dark")
            Application.Current.UserAppTheme = Microsoft.Maui.ApplicationModel.AppTheme.Dark;
        else
            Application.Current.UserAppTheme = Microsoft.Maui.ApplicationModel.AppTheme.Unspecified;
    }
}
