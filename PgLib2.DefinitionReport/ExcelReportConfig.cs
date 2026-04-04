namespace PgLib2.DefinitionReport;

public class ExcelReportConfig
{
    public string NameCell { get; set; } = string.Empty;
    public string CommentCell { get; set; } = string.Empty;
    public int ColumnStartRow { get; set; }
    public string ColumnNameCol { get; set; } = string.Empty;
    public string DataTypeCol { get; set; } = string.Empty;
    public string LengthCol { get; set; } = string.Empty;
    public string NotNullCol { get; set; } = string.Empty;
    public string PrimaryKeyCol { get; set; } = string.Empty;
    public string DefaultCol { get; set; } = string.Empty;
    public string ColumnCommentCol { get; set; } = string.Empty;
    public int IndexStartRow { get; set; }
    public string IndexCol { get; set; } = string.Empty;
    public int DefinitionStartRow { get; set; }
    public string DefinitionCol { get; set; } = string.Empty;
}
