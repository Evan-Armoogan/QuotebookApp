using System.Net.Http.Json;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Sheets.v4;
using Google.Apis.Services;
using Google.Apis.Sheets.v4.Data;

namespace QuotebookApp.Services;

class UploadValues
{
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

    private void retry(int retries)
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
    }

    private UploadValues ConstructUploadObject(string range, string major_dimension, string[] value_array)
    {
        UploadValues vals = new UploadValues();
        vals.range = range;
        vals.majorDimension = major_dimension;
        string[][] values = new string[value_array.Length][];
        values[0] = value_array;
        vals.values = values;
        return vals;
    }


    public async Task<SheetData> GetResponse(string range)
    {
        string base_url = GlobalData.BaseURL;
        string spreadsheet_id = GlobalData.SheetID;
        string major_dimension = "ROWS";
        
        var url = $"{base_url}/{spreadsheet_id}/values/{range}?majorDimension={major_dimension}";

        int retries = 0;
        while (true)
        {
            using HttpResponseMessage response = await service.HttpClient.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                SheetData data = await response.Content.ReadAsAsync<SheetData>();
                return data;
            }

            else if (GlobalData.RetryStatusCodes.Contains(Convert.ToInt32(response.StatusCode)))
            {
                // valid retry request. Times based on Google Sheets API Quota Limits
                // should improve this, just want working code for now
                retry(retries);
                retries++;
            }

            else
            {
                throw new Exception($"API request failed due to unsuccessful response code: {response.StatusCode}");
            }
        }
    }

    public async Task SetResponse(string range, string[] value_array)
    {
        string base_url = GlobalData.BaseURL;
        string spreadsheet_id = GlobalData.SheetID;
        string major_dimension = "ROWS";
        string value_input_option = "USER_ENTERED";
        string insert_data_option = "INSERT_ROWS";

        var url = $"{base_url}/{spreadsheet_id}/values/{range}:append?valueInputOption={value_input_option}&insertDataOption={insert_data_option}";

        UploadValues vals = ConstructUploadObject(range, major_dimension, value_array);

        JsonContent json_content = JsonContent.Create<UploadValues>(vals);
        HttpContent content = (HttpContent)json_content;
        var uri = new Uri(url);

        int retries = 0;
        while (true)
        {
            using HttpResponseMessage response = await service.HttpClient.PostAsync(uri, content);
            if (response.IsSuccessStatusCode)
            {
                PostSheetData data = await response.Content.ReadAsAsync<PostSheetData>();
                if (data.spreadsheetID != spreadsheet_id)
                {
                    // request unsuccessful, invalid response
                    throw new Exception("Update unsuccessful: invalid API response. Please try again.");
                }

                return;
            }

            else if (GlobalData.RetryStatusCodes.Contains(Convert.ToInt32(response.StatusCode)))
            {
                // valid retry request. Times based on Google Sheets API Quota Limits
                retry(retries);
                retries++;
            }

            else
            {
                throw new Exception($"API request failed due to unsuccessful response code: {response.StatusCode}");
            }
        }
    }

    public async Task EditResponse(string range, string[] value_array)
    {
        string base_url = GlobalData.BaseURL;
        string spreadsheet_id = GlobalData.SheetID;
        string major_dimension = "ROWS";
        string value_input_option = "USER_ENTERED";

        var url = $"{base_url}/{spreadsheet_id}/values/{range}?valueInputOption={value_input_option}";

        UploadValues vals = ConstructUploadObject(range, major_dimension, value_array);

        JsonContent json_content = JsonContent.Create<UploadValues>(vals);
        HttpContent content = (HttpContent)json_content;
        var uri = new Uri(url);

        int retries = 0;
        while (true)
        {
            using HttpResponseMessage response = await service.HttpClient.PutAsync(uri, content);
            if (response.IsSuccessStatusCode)
            {
                PostSheetData data = await response.Content.ReadAsAsync<PostSheetData>();
                if (data.spreadsheetID != spreadsheet_id)
                {
                    // request unsuccessful, invalid response
                    throw new Exception("Update unsuccessful: invalid API response. Please try again.");
                }

                return;
            }

            else if (GlobalData.RetryStatusCodes.Contains(Convert.ToInt32(response.StatusCode)))
            {
                // valid retry request. Times based on Google Sheets API Quota Limits
                retry(retries);
                retries++;
            }

            else
            {
                throw new Exception($"API request failed due to unsuccessful response code: {response.StatusCode}");
            }
        }
    }
}

