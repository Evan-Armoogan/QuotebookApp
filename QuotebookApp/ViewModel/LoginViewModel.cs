namespace QuotebookApp.ViewModel;

public partial class LoginViewModel : BaseViewModel
{
    UserService userService;
    AppFlagService flagService;

    [ObservableProperty]
    string username;

    [ObservableProperty]
    string password;

    [ObservableProperty]
    bool loginInvalid;

    public LoginViewModel(UserService userService, AppFlagService flagService)
    {
        Title = "Login";
        IsLoggedIn = GlobalData.IsLoggedIn;
        IsBusy = false;
        this.userService = userService;
        this.flagService = flagService;

        /* Initialize Runtime Data now, this page is opened once immediately at startup */
        Initialization.InitializeClientHeight();
    }

    [RelayCommand]
    async Task GoToQuotePageAsync()
    {
        await Shell.Current.GoToAsync(nameof(QuotePage));
    }

    [RelayCommand]
    async Task GoToSettingsPageAsync()
    {
        await Shell.Current.GoToAsync(nameof(SettingsPage));
    }

    [RelayCommand]
    async Task GoToQOTDPageAsync()
    {
        await Shell.Current.GoToAsync(nameof(QotdPage));
    }

    [RelayCommand]
    async Task GoToReaderPageAsync()
    {
        await Shell.Current.GoToAsync(nameof(ReaderPage));
    }

    [RelayCommand]
    void Logout()
    {
        IsLoggedIn = false;
        GlobalData.IsLoggedIn = false;
        GlobalData.CurrentUser = null;
        userService.ClearUsers();
        Title = "Login";
    }

    [RelayCommand]
    async Task LoginAsync()
    {
        /* First, call sheets API to get login data */

        List<User> users = new List<User>();

        if (IsBusy)
            return;
        try
        {
            IsBusy = true;

            users = await userService.GetUsers();
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.ToString());
            await Shell.Current.DisplayAlert("Error!", $"Unable to retrieve list of registered users: {ex.Message}", "OK");
        }

        foreach (User user in users)
        {
            if (user.UserName == Username && user.UserPass == Password)
            {
                /* Good login details */
                GlobalData.CurrentUser = user;

                /* Check if the user is allowed to log in (i.e. app is not down) */
                bool loginAllowed = await flagService.IsLoginAllowed();
                if (!loginAllowed)
                {
                    await Shell.Current.DisplayAlert("Error!", "The app is currently down for maintenance. Try again later.", "OK");
                    GlobalData.CurrentUser = null;
                    Password = "";
                    IsBusy = false;
                    return;
                }

                /* All good, process user details and set up home page */
                GlobalData.IsLoggedIn = true;
                IsLoggedIn = true;
                LoginInvalid = false;
                Title = "Home";

                // empty username and password fields
                Username = "";
                Password = "";

                break;
            }
        }

        if (!GlobalData.IsLoggedIn)
        {
            GlobalData.CurrentUser = null;
            LoginInvalid = true;

            // empty password field
            Password = "";
        }

        IsBusy = false;
        GlobalData.Users = users;
    }
}