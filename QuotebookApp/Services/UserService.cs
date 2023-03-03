using System.Globalization;
using System.Net.Http;
using System.Net.Http.Headers;
using QuotebookApp.Services;

namespace QuotebookApp.Services;

public class UserService
{
    BaseService service;

    public UserService()
    {
        service = new BaseService();
    }

    List<User> userList = new List<User>();
    public async Task<List<User>> GetUsers()
    {
        if (userList?.Count > 0)
            return userList;

        string range = GlobalData.UserSheetName + "!A2:C";

        SheetData data = await service.GetResponse(range);

        foreach (string[] value in data.Values)
        {
            User user = new User(Convert.ToInt32(value[0]), value[1], value[2]);
            userList.Add(user);
        }

        return userList;
    }
}