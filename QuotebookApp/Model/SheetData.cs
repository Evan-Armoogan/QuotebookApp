namespace QuotebookApp.Model;

public class SheetData
{
    public string Range { get; set; }
    public enum MajorDimension { DIMENSION_UNSPECIFIED, ROWS, COLUMNS };
    public string[][] Values { get; set; }
}

public class PostSheetData
{
    public string spreadsheetID { get; set; }
    public string tableRange { get; set; }
}