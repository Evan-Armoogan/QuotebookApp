using System.Net.Http.Json;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Sheets.v4;
using Google.Apis.Services;
using Google.Apis.Sheets.v4.Data;

namespace QuotebookApp.Services;

class UploadValues
{
    public UploadValues(string range, string majorDimension, string[][] values)
    {
        this.range = range;
        this.majorDimension = majorDimension;
        this.values = values;
    }

    public string range { get; set; }
    public string majorDimension { get; set; }
    public string[][] values { get; set; }
}


public class BaseSheetService
{
    public static SheetsService service;

    public async static void InitializeClient()
    {
        string[] scopes = { SheetsService.Scope.Spreadsheets };
        service = await BaseService.InitializeService<SheetsService>(scopes);
    }

    private void retry(ref int retries)
    {
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

    private async Task apiRequest(Func<Task<HttpResponseMessage>> callFn, Func<HttpResponseMessage, Task> successHandling)
    {
        int retries = 0;
        while (true)
        {
            using HttpResponseMessage response = await callFn();
            if (response.IsSuccessStatusCode)
            {
                await successHandling(response);
                break;
            }

            else if (GlobalData.RetryStatusCodes.Contains(Convert.ToInt32(response.StatusCode)))
            {
                /* valid retry request. Times based on Google Sheets API Quota Limits
                 * should improve this, just want working code for now */
                retry(ref retries);
            }

            else
            {
                throw new Exception($"API request failed due to unsuccessful response code: {response.StatusCode}");
            }
        }
    }

    public async Task<SheetData> GetResponse(string range)
    {
        string base_url = GlobalData.BaseURL;
        string spreadsheet_id = GlobalData.SheetID;
        string major_dimension = "ROWS";

        var url = $"{base_url}/{spreadsheet_id}/values/{range}?majorDimension={major_dimension}";

        SheetData data = new SheetData();
        await apiRequest(async () => await service.HttpClient.GetAsync(url), async (HttpResponseMessage response) => data = await response.Content.ReadAsAsync<SheetData>());

        return data;
    }

    private async Task setEditRequest(Func<Uri, HttpContent, Task<HttpResponseMessage>> callFn, string url, string range, string[][] value_array)
    {
        string major_dimension = "ROWS";
        UploadValues vals = new UploadValues(range, major_dimension, value_array);

        JsonContent json_content = JsonContent.Create<UploadValues>(vals);
        HttpContent content = (HttpContent)json_content;
        Uri uri = new Uri(url);

        await apiRequest(async() => await callFn(uri, content), async (HttpResponseMessage response) =>
        {
            PostSheetData data = await response.Content.ReadAsAsync<PostSheetData>();
            if (data.spreadsheetID != GlobalData.SheetID)
            {
                // request unsuccessful, invalid response
                throw new Exception("Update unsuccessful: invalid API response. Please try again.");
            }
        });
    }

    public async Task SetResponse(string range, string[][] value_array)
    {
        string base_url = GlobalData.BaseURL;
        string spreadsheet_id = GlobalData.SheetID;
        string value_input_option = "USER_ENTERED";
        string insert_data_option = "INSERT_ROWS";

        var url = $"{base_url}/{spreadsheet_id}/values/{range}:append?valueInputOption={value_input_option}&insertDataOption={insert_data_option}";

        await setEditRequest(service.HttpClient.PostAsync, url, range, value_array);
    }

    public async Task EditResponse(string range, string[][] value_array)
    {
        string base_url = GlobalData.BaseURL;
        string spreadsheet_id = GlobalData.SheetID;
        string value_input_option = "USER_ENTERED";

        var url = $"{base_url}/{spreadsheet_id}/values/{range}?valueInputOption={value_input_option}";

        await setEditRequest(service.HttpClient.PutAsync, url, range, value_array);
    }
}
