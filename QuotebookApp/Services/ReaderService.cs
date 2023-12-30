namespace QuotebookApp.Services;

public class ReaderService
{
    BaseSheetService service;

    public ReaderService()
    {
        service = new BaseSheetService();
    }

    public async Task<List<ReaderFileInfo>> GetAvailableFiles()
    {
        string range = "Reader!A2:D";
        SheetData data = await service.GetResponse(range);

        List<ReaderFileInfo> files = new List<ReaderFileInfo>();

        if (data.Values is null)
            return files;

        foreach (string[] value in data.Values)
        {
            ReaderFileInfo fileinfo = new ReaderFileInfo(value[0], value[1], value[2], Convert.ToInt32(value[3]));
            files.Add(fileinfo);
        }

        return files;
    }
}
