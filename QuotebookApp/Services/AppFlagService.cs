namespace QuotebookApp.Services;

public class AppFlagService
{
    BaseSheetService service;
    public AppFlagService()
    {
        service = new BaseSheetService();
    }

    public async Task<bool> IsLoginAllowed()
    {
        string range = "AppFlag!A1";
        SheetData data = await service.GetResponse(range);

        if (data.Values is null)
            return true;

        if (data.Values[0][0] != "Y")
            return true;

        /* Admins can always access the app */
        if (GlobalData.CurrentUser.UserType == User.UserPermissionType.Admin)
            return true;

        return false;
    }
}
