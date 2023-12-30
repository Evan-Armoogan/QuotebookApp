namespace QuotebookApp.Model;

public class ReaderFileInfo
{
    public ReaderFileInfo(string name, string friendlyName, string id, int size)
    {
        Name = name;
        FriendlyName = friendlyName;
        ID = id;
        Size = size;
    }

    public string Name { get; set; }
    public string FriendlyName { get; set; }
    public string ID { get; set; }
    public int Size { get; set; }
}
