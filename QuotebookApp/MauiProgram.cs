using Microsoft.Extensions.Logging;
using QuotebookApp.Services;
using QuotebookApp.View;

namespace QuotebookApp;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});

#if DEBUG
		builder.Logging.AddDebug();
#endif

		builder.Services.AddSingleton<UserService>();
		builder.Services.AddSingleton<QuoteService>();

		builder.Services.AddSingleton<LoginViewModel>();
        /* We want this to be transient so the page will load new quotes each time it's opened */
        builder.Services.AddTransient<QuoteViewModel>();
		builder.Services.AddTransient<SettingsViewModel>();

		builder.Services.AddSingleton<LoginPage>();
		builder.Services.AddTransient<QuotePage>();
		builder.Services.AddTransient<SettingsPage>();



		return builder.Build();
	}
}
