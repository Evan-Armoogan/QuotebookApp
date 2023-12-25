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

        try
        {
            IsBusy = true;
            SelectedTheme = GlobalData.SetAppTheme(SelectedTheme);
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.ToString());
            await Shell.Current.DisplayAlert("Error!", $"Unable to update App Theme: {ex.Message}", "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }
}

