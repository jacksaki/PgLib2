namespace PgLib2.Objects;

[AttributeUsage(AttributeTargets.Field)]
internal class TableTypeAttribute : Attribute
{
    public string Name { get; }
    public string RelKind { get; }
    public TableTypeAttribute(string name, string relKind)
    {
        this.Name = name;
        this.RelKind = relKind;
    }
}
