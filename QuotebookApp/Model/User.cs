namespace QuotebookApp.Model;

public class User
{
    public User(int userID, string userName, string userPass)
    {
        UserID = userID;
        UserName = userName;
        UserPass = userPass;
    }

    public int UserID { get; set; }
    public string UserName { get; set; }
    public string UserPass { get; set; }
}