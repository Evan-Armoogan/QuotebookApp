using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;

namespace QuotebookApp;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>().UseMauiCommunityToolkit()
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
		builder.Services.AddSingleton<ReaderService>();
		builder.Services.AddSingleton<BaseDriveService>();
		builder.Services.AddSingleton<AppFlagService>();

		builder.Services.AddSingleton<LoginViewModel>();
        /* We want this to be transient so the page will load new quotes each time it's opened */
        builder.Services.AddTransient<QuoteViewModel>();
		builder.Services.AddTransient<QotdViewModel>();
		builder.Services.AddTransient<ReaderViewModel>();
		builder.Services.AddTransient<PdfReaderViewModel>();
		builder.Services.AddTransient<SettingsViewModel>();
		builder.Services.AddTransient<ChangelogViewModel>();

		builder.Services.AddSingleton<LoginPage>();
		builder.Services.AddTransient<QuotePage>();
		builder.Services.AddTransient<QotdPage>();
		builder.Services.AddTransient<ReaderPage>();
		builder.Services.AddTransient<PdfReaderPage>();
		builder.Services.AddTransient<SettingsPage>();
		builder.Services.AddTransient<ChangelogPage>();



		return builder.Build();
	}
}
