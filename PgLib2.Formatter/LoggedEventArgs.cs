namespace PgLib2.Formatter;

public enum InstallLogItemType
{
    Output,
    Error,
}
public class LogItem
{
    public string FormattedMessage => $"{this.Date:HH:mm:ss}\t{this.Type}\t{this.Message}";
    public LogItem(InstallLogItemType type, string message)
    {
        this.Date = DateTime.Now;
        this.Message = message;
        this.Type = type;
    }
    public InstallLogItemType Type { get; }
    public string Message { get; }
    public DateTime Date { get; }
}
