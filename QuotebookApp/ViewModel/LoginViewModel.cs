using QuotebookApp.Services;

namespace QuotebookApp.ViewModel;

public partial class LoginViewModel : BaseViewModel
{
    UserService userService;
    public ObservableCollection<User> Users { get; } = new ObservableCollection<User>();

    public LoginViewModel(UserService userService)
    {
        Title = "Login";
        IsLoggedIn = GlobalData.IsLoggedIn;
        this.userService = userService;
    }


    [RelayCommand]
    async Task GetUsersAsync()
    {
        if (IsBusy)
            return;
        try
        {
            IsBusy = true;
            var users = await userService.GetUsers();

            if (Users.Count != 0)
                Users.Clear();

            foreach (var user in users)
                Users.Add(user);
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.ToString());
            await Shell.Current.DisplayAlert("Error!", $"Unable to retrieve list of registered users: {ex.Message}", "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }
}