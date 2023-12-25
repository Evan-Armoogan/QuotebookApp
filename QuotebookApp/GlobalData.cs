namespace QuotebookApp;

public class GlobalData
{

    public static string BaseURL { get; } = "https://sheets.googleapis.com/v4/spreadsheets";
    public static string SheetID { get; private set; }
    public static List<int> RetryStatusCodes { get; private set; }
    public static User CurrentUser { get; set; }
    public static List<User> Users { get; set; }
    public static int ClientHeight { get; private set; }


    public static bool IsLoggedIn { get; set; } = false;

    private const int client_height_offset = 140;


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

    private static void resetFile(string filename)
    {
        FileStream streamReset = File.OpenWrite(filename);
        streamReset.SetLength(0);
        streamReset?.Close();
    }

    private static string initTheme(bool write_theme, string theme="Device Default")
    {
        string SetTheme = "Device Default";

        string[] themes = { "Light", "Dark", "Device Default" };
        string directory = $"{FileSystem.AppDataDirectory}/";
        string target = Path.Combine(directory, "AppTheme.txt");

        /* if we want to read the data, we will do so only if the file exists */
        if (File.Exists(target) && !write_theme)
        {
            FileStream stream = File.OpenRead(target);
            StreamReader reader = new StreamReader(stream);
            string result = reader.ReadToEnd();
            SetTheme = result;

            reader?.Close();
            stream?.Close();

            if (!themes.Contains(result))
            {
                /* Bad data, empty file and rewrite default */
                write_theme = true;
                theme = "Device Default";
            }
        }

        /* if data not read or we want to overwrite, write given theme to file */
        if (write_theme)
        {
            if (File.Exists(target))
                resetFile(target);

            FileStream stream = File.OpenWrite(target);
            StreamWriter writer = new StreamWriter(stream);
            writer.Write(theme);
            SetTheme = theme;
            writer?.Close();
            stream?.Close();
        }

        if (SetTheme == "Light")
            Application.Current.UserAppTheme = Microsoft.Maui.ApplicationModel.AppTheme.Light;
        else if (SetTheme == "Dark")
            Application.Current.UserAppTheme = Microsoft.Maui.ApplicationModel.AppTheme.Dark;
        else
            Application.Current.UserAppTheme = Microsoft.Maui.ApplicationModel.AppTheme.Unspecified;

        return SetTheme;
    }

    public static void InitializeAppTheme()
    {
        initTheme(false);
    }

    public static string SetAppTheme(string NewTheme)
    {
        return initTheme(true, NewTheme);
    }
}
