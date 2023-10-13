using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Sheets.v4;
using Google.Apis.Services;
using Google.Apis.Sheets.v4.Data;
using Newtonsoft.Json;

namespace QuotebookApp.Services;

class QuoteUploadValues
{
    public string range { get; set; }
    public string majorDimension { get; set; }
    public string[][] values { get; set; }
}


public class BaseService
{
    public static SheetsService service;

    public async static void InitializeClient()
    {
        using var stream_app = await FileSystem.OpenAppPackageFileAsync("Secrets/ApplicationName.secret");
        using var reader_app = new StreamReader(stream_app);
        string app_name = await reader_app.ReadToEndAsync();
        reader_app.Close();
        stream_app?.Close();

        string[] scopes = { SheetsService.Scope.Spreadsheets };
        GoogleCredential credential = GoogleCredential.FromJson(ServiceAccount.GetServAcc()).CreateScoped(scopes);

        service = new SheetsService(new BaseClientService.Initializer()
        {
            HttpClientInitializer = credential,
            ApplicationName = app_name
        });
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
    }

    public async Task SetResponse(string range, string[] value_array)
    {
        string base_url = GlobalData.BaseURL;
        string spreadsheet_id = GlobalData.SheetID;
        string major_dimension = "ROWS";
        string value_input_option = "USER_ENTERED";
        string insert_data_option = "INSERT_ROWS";

        var url = $"{base_url}/{spreadsheet_id}/values/{range}:append?valueInputOption={value_input_option}&insertDataOption={insert_data_option}";

        QuoteUploadValues vals = new QuoteUploadValues();
        vals.range = range;
        vals.majorDimension = major_dimension;
        string[][] values = new string[value_array.Length][];
        values[0] = value_array;
        vals.values = values;

        JsonContent json_content = JsonContent.Create<QuoteUploadValues>(vals);
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
    }
}

