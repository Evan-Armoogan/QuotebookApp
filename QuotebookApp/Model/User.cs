namespace QuotebookApp.Model;

public class User
{
    public enum UserPermissionType
    {
        Admin = 0,
        Normal,
        Invisible,
    }

    public User(int userID, string userName, string userPass, UserPermissionType userType)
    {
        UserID = userID;
        UserName = userName;
        UserPass = userPass;
        UserType = userType;
    }

    public int UserID { get; set; }
    public string UserName { get; set; }
    public string UserPass { get; set; }
    public UserPermissionType UserType { get; set; }
}
