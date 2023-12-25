namespace QuotebookApp;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();

		Routing.RegisterRoute(nameof(QuotePage), typeof(QuotePage));
		Routing.RegisterRoute(nameof(QotdPage), typeof(QotdPage));
		Routing.RegisterRoute(nameof(SettingsPage), typeof(SettingsPage));
	}
}
