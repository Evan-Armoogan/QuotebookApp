namespace QuotebookApp;

public class AppTheme
{
    private static void resetFile(string filename)
    {
        FileStream streamReset = File.OpenWrite(filename);
        streamReset.SetLength(0);
        streamReset?.Close();
    }

    private static string initTheme(bool write_theme, string theme = "Device Default")
    {
        string SetTheme = "Device Default";

        string[] themes = { "Light", "Dark", "Device Default" };
        string directory = $"{FileSystem.Current.AppDataDirectory}/";
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
