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
		builder.Services.AddSingleton<QuoteViewModel>();

		builder.Services.AddSingleton<LoginPage>();
		builder.Services.AddSingleton<QuotePage>();



		return builder.Build();
	}
}
