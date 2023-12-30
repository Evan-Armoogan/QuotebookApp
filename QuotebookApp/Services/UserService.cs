namespace QuotebookApp.Services;

public class UserService
{
    BaseSheetService service;

    public UserService()
    {
        service = new BaseSheetService();
    }

    List<User> userList = new List<User>();
    
    public void ClearUsers()
    {
        userList.Clear();
    }

    public async Task<List<User>> GetUsers()
    {
        ClearUsers();

        string range = "Users!A2:D";

        SheetData data = await service.GetResponse(range);

        foreach (string[] value in data.Values)
        {
            string type = value[3];
            User.UserPermissionType permType;
            switch (type)
            {
                case "A":
                    permType = User.UserPermissionType.Admin;
                    break;
                case "N":
                    permType = User.UserPermissionType.Normal;
                    break;
                case "I":
                    permType = User.UserPermissionType.Invisible;
                    break;
                default:
                    permType = User.UserPermissionType.Normal;
                    break;
            }

            User user = new User(Convert.ToInt32(value[0]), value[1], value[2], permType);
            userList.Add(user);
        }

        return userList;
    }

    public async Task SetUserPassword(string password)
    {
        int user_id = GlobalData.CurrentUser.UserID;
        int sheet_row = user_id + 1;
        string range = $"Users!C{sheet_row}";

        string[] values = { password };

        await service.EditResponse(range, values);
    }
}
