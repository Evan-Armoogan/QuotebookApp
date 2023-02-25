namespace QuotebookApp;

public partial class App : Application
{
	public App()
	{
		InitializeComponent();

		GlobalData.InitializeApiParameters();
		GlobalData.IniitalizeRetryStatusCodes();

		MainPage = new AppShell();
	}
}
