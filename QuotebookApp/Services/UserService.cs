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
        int user = GlobalData.Users.FindIndex(u => (u.UserID == GlobalData.CurrentUser.UserID));
        string range = $"Users!C{user+2}";

        string[] values = { password };
        string[][] values_send = new string[1][];
        values_send[0] = values;

        await service.EditResponse(range, values_send);
    }
}
