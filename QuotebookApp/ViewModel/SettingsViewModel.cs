using QuotebookApp.Services;

namespace QuotebookApp.ViewModel;
public partial class SettingsViewModel : BaseViewModel
{
    UserService userService;

    [ObservableProperty]
    string username;

    [ObservableProperty]
    string password;

    [ObservableProperty]
    string newPassword;

    [ObservableProperty]
    string confirmPassword;

    [ObservableProperty]
    bool isValidPassword;

    [ObservableProperty]
    string selectedTheme;

    private void ResetPasswordFields()
    {
        Password = "";
        NewPassword = "";
        ConfirmPassword = "";
    }

    public SettingsViewModel(UserService userService)
    {
        Title = "Settings";
        IsBusy = false;
        IsValidPassword = false;
        Username = GlobalData.CurrentUser.UserName;
        ResetPasswordFields();
        this.userService = userService;
    }

    [RelayCommand]
    public void CheckPasswordValidity()
    {
        if ((Password == GlobalData.CurrentUser.UserPass) && (NewPassword == ConfirmPassword) && (NewPassword.Length >= 4))
            IsValidPassword = true;
        else
        {
            IsValidPassword = false;
        }
    }

    [RelayCommand]
    public async Task UpdatePasswordAsync()
    {
        if (IsBusy)
            return;

        CheckPasswordValidity();
        if (!IsValidPassword)
            return;

        try
        {
            IsBusy = true;
            await userService.SetUserPassword(NewPassword);
            await Shell.Current.DisplayAlert("Success", "Password updated", "OK");
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.ToString());
            await Shell.Current.DisplayAlert("Error!", $"Unable to update password: {ex.Message}", "OK");
        }
        finally
        {
            IsBusy = false;
            IsValidPassword = false;
            ResetPasswordFields();
        }
    }

    [RelayCommand]
    public async Task UpdateThemeAsync()
    {
        if (IsBusy)
            return;

        string default_theme = "Device Default";
        string directory = $"{FileSystem.AppDataDirectory}/";
        string target = Path.Combine(directory, "AppTheme.txt");

        if (File.Exists(target))
        {
            /* First empty the file, then write new theme in */
            FileStream streamReset = File.OpenWrite(target);
            streamReset.SetLength(0);
            streamReset.Close(); // ensures file is wiped
            FileStream streamWrite = File.OpenWrite(target);
            StreamWriter writer = new StreamWriter(streamWrite);
            await writer.WriteAsync(SelectedTheme);
            writer?.Close();
            streamWrite?.Close();
            GlobalData.AppTheme = SelectedTheme;
        }
        else
        {
            using FileStream stream = File.OpenWrite(target);
            using StreamWriter writer = new StreamWriter(stream);
            await writer.WriteAsync(default_theme);
            GlobalData.AppTheme = default_theme;
            stream?.Close();
            writer?.Close();
        }

        if (GlobalData.AppTheme == "Light")
            Application.Current.UserAppTheme = AppTheme.Light;
        else if (GlobalData.AppTheme == "Dark")
            Application.Current.UserAppTheme = AppTheme.Dark;
        else
            Application.Current.UserAppTheme = AppTheme.Unspecified;
    }
}

