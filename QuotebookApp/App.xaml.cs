using QuotebookApp.Services;

namespace QuotebookApp;

public partial class App : Application
{
	public App()
	{
		InitializeComponent();

		GlobalData.InitializeApiParameters();
		GlobalData.IniitalizeRetryStatusCodes();
		BaseService.InitializeClient();

		MainPage = new AppShell();
	}
}
