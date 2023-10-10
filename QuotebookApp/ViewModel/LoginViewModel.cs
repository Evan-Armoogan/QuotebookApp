using QuotebookApp.Services;
using System.Linq;

namespace QuotebookApp.ViewModel;

public partial class LoginViewModel : BaseViewModel
{
    UserService userService;


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
    async Task GoToQuotePageAsync()
    {
        await Shell.Current.GoToAsync(nameof(QuotePage));
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