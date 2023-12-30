using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;

namespace QuotebookApp.Services;

public class BaseService
{
    public async static Task<T> InitializeService<T>(string[] scopes)
    {
        using var stream_app = await FileSystem.OpenAppPackageFileAsync("Secrets/ApplicationName.secret");
        using var reader_app = new StreamReader(stream_app);
        string app_name = await reader_app.ReadToEndAsync();
        reader_app.Close();
        stream_app?.Close();

        GoogleCredential credential = GoogleCredential.FromJson(ServiceAccount.GetServAcc()).CreateScoped(scopes);

        T service = (T)Activator.CreateInstance(typeof(T), new BaseClientService.Initializer()
        {
            HttpClientInitializer = credential,
            ApplicationName = app_name
        });

        return service;
    }
}
