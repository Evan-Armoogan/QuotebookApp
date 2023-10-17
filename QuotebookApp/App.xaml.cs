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

#if WINDOWS || MACCATALYST
    protected override Window CreateWindow(IActivationState activationState)
    {
        Window window = base.CreateWindow(activationState);

        window.MinimumHeight = 760;
        window.MaximumHeight = 760;
        window.Height = 760;
        window.MinimumWidth = 1440;
        window.MaximumWidth = 1440;
        window.Width = 1440;

        return window;
    }
#endif
}
