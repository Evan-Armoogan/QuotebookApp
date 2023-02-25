using System.Globalization;
using System.Net.Http.Json;
using static System.Net.WebRequestMethods;

namespace QuotebookApp.Services;

public class UserService
{
    HttpClient httpClient;
    public UserService()
    {
        httpClient = new HttpClient();
    }

    List<User> userList = new List<User>();
    public async Task<List<User>> GetUsers()
    {
        if (userList?.Count > 0)
            return userList;


        string base_url = GlobalData.BaseURL;
        string spreadsheet_id = GlobalData.SheetID;
        string range = GlobalData.UserSheetName + "!A2:C";
        string major_dimension = "ROWS";
        string key = GlobalData.APIKey;

        var url = $"{base_url}/{spreadsheet_id}/values/{range}?majorDimension={major_dimension}&key={key}";

        int retries = 0;
        while (true)
        {
            var response = await httpClient.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                userList = await response.Content.ReadFromJsonAsync<List<User>>();
                break;
            }

            else if (GlobalData.RetryStatusCodes.Contains(Convert.ToInt32(response.StatusCode)))
            {
                // valid retry request. Times based on Google Sheets API Quota Limits

                switch (retries)
                {
                    case 0:
                        Thread.Sleep(1000);
                        break;
                    case 1:
                        Thread.Sleep(5000);
                        break;
                    case 2:
                        Thread.Sleep(10000);
                        break;
                    case 3:
                        Thread.Sleep(15000);
                        break;
                    case 4:
                        Thread.Sleep(30000);
                        break;
                    default:
                        throw new Exception($"API request timed out. Try again later.");
                }

                retries++;
            }

            else
            {
                throw new Exception($"API request failed due to unsuccessful response code: {response.StatusCode}");
            }
        }

        return userList;
    }
}