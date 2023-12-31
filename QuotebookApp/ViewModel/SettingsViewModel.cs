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
    void CheckPasswordValidity()
    {
        if ((Password == GlobalData.CurrentUser.UserPass) && (NewPassword == ConfirmPassword) && (NewPassword.Length >= 4))
            IsValidPassword = true;
        else
        {
            IsValidPassword = false;
        }
    }

    [RelayCommand]
    async Task UpdatePasswordAsync()
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
            GlobalData.CurrentUser.UserPass = NewPassword;
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
            /* Update button state appropriately.
             * There seems to be an issue with setting the button state
             * via data binding of the button calling the command while
             * the command is being executed. To fix this I just ran on a
             * separate thread code to disable the delete button after a slight
             * delay to ensure this command has completed running by the time the
             * button is disabled. Really annoying, hopefully it gets fixed soon.
             * TODO: change this line to IsValidPassword = false when community
             * toolkit bug is fixed.
             */
            _ = Task.Run(() => { Thread.Sleep(1); IsValidPassword = false; });
            ResetPasswordFields();
        }
    }

    [RelayCommand]
    async Task UpdateThemeAsync()
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;
            SelectedTheme = AppTheme.SetAppTheme(SelectedTheme);
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

    [RelayCommand]
    async Task GoToChangelogPageAsync()
    {
        await Shell.Current.GoToAsync(nameof(ChangelogPage));
    }
}

