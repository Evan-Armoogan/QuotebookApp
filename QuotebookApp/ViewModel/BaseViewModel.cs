using CommunityToolkit.Mvvm.ComponentModel;

namespace QuotebookApp.ViewModel;

public partial class BaseViewModel : ObservableObject
{
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsNotBusy))]
    bool isBusy;

    [ObservableProperty]
    string title;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsNotLoggedIn))]
    bool isLoggedIn;

    public bool IsNotBusy => !IsBusy;
    public bool IsNotLoggedIn => !IsLoggedIn;
}
