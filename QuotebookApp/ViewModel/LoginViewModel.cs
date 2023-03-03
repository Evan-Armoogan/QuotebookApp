using QuotebookApp.Services;

namespace QuotebookApp.ViewModel;

public partial class LoginViewModel : BaseViewModel
{
    UserService userService;

    // don't think I need this here, but will in quote page.
    //public ObservableCollection<User> Users { get; } = new ObservableCollection<User>();

    [ObservableProperty]
    string username;

    [ObservableProperty]
    string password;

    [ObservableProperty]
    bool loginInvalid;

    public LoginViewModel(UserService userService)
    {
        Title = "Login";
        IsLoggedIn = GlobalData.IsLoggedIn;
        this.userService = userService;
    }


    [RelayCommand]
    async Task LoginAsync()
    {
        // First, call sheets API to get login data

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
                // logged in
                GlobalData.CurrentUser = user;
                GlobalData.IsLoggedIn = true;
                IsLoggedIn = true;
                LoginInvalid = false;
            }
        }

        if (!GlobalData.IsLoggedIn)
        {
            GlobalData.CurrentUser = null;
            LoginInvalid = true;
        }

        IsBusy = false;
    }
}