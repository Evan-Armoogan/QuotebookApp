using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace QuotebookApp.Services;

public class BaseService
{
    public static HttpClient httpClient;

    public static void InitializeClient()
    {
        httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Accept.Clear();
        httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    }


    public async Task<SheetData> GetResponse(string range)
    {
        string base_url = GlobalData.BaseURL;
        string spreadsheet_id = GlobalData.SheetID;
        string major_dimension = "ROWS";
        string key = GlobalData.APIKey;

        var url = $"{base_url}/{spreadsheet_id}/values/{range}?majorDimension={major_dimension}&key={key}";

        int retries = 0;
        while (true)
        {
            using HttpResponseMessage response = await BaseService.httpClient.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                SheetData data = await response.Content.ReadAsAsync<SheetData>();
                return data;
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

