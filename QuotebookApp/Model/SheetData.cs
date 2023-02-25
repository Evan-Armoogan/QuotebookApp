namespace QuotebookApp.Model;

public class SheetData
{
    public string Range { get; set; }
    public enum MajorDimension { DIMENSION_UNSPECIFIED, ROWS, COLUMNS };
    public User[] Values { get; set; }
}