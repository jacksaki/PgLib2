using PgLib2.Objects.Query;

namespace PgLib2.Objects;

public class PgColumn
{
    public static SQLSet GetSQLSet() => PgColumnQuery.GenerateSQLSet();
    internal PgColumn(PgCatalog catalog)
    {
        _catalog = catalog;
    }
    private PgCatalog _catalog;

    private string GetDataType()
    {
        if (this.CharacterMaximumLength.HasValue)
        {
            return $"{this.DataType}({this.CharacterMaximumLength})";
        }
        else if (this.DataType.Equals("numeric"))
        {
            if (this.NumericPrecision.HasValue && this.NumericScale.HasValue)
            {
                return $"{this.DataType}({this.NumericPrecision},{this.NumericScale})";
            }
            else if (this.NumericPrecision.HasValue)
            {
                return $"{this.DataType}({this.NumericPrecision})";
            }
            else
            {
                return this.DataType;
            }
        }
        else
        {
            return this.DataType;
        }
    }
    public string GenerateColumnDDL()
    {
        var sb = new System.Text.StringBuilder();
        sb.Append($"{this.ColumnName}");
        sb.Append($" {this.GetDataType()}");

        if (!this.DataType.Contains("serial") && !this.DataType.Contains("AS IDENTITY") && this.ColumnDefault != null)
        {
            sb.Append($" DEFAULT {this.ColumnDefault}");
        }
        if (this.IsNotNull)
        {
            sb.Append(" NOT NULL");
        }
        return sb.ToString();
    }

    [DbColumn("table_oid")]
    internal uint TableOid { get; private set; }

    [DbColumn("table_schema")]
    public string TableSchema { get; private set; } = string.Empty;

    [DbColumn("need_quote")]
    public bool NeedQuote { get; private set; }

    [DbColumn("table_name")]
    public string TableName { get; private set; } = string.Empty;

    [DbColumn("column_name")]
    public string ColumnName { get; private set; } = string.Empty;

    [DbColumn("ordinal_position")]
    public short OrdinalPosition { get; private set; }

    [DbColumn("column_default")]
    public string ColumnDefault { get; private set; } = string.Empty;

    [DbColumn("is_not_null")]
    public bool IsNotNull { get; private set; }

    [DbColumn("data_type")]
    public string DataType { get; private set; } = string.Empty;

    [DbColumn("character_maximum_length")]
    public int? CharacterMaximumLength { get; private set; }

    [DbColumn("numeric_precision")]
    public int? NumericPrecision { get; private set; }

    [DbColumn("numeric_scale")]
    public int? NumericScale { get; private set; }

    [DbColumn("datetime_precision")]
    public int? DateTimePrecision { get; private set; }

    [DbColumn("is_updatable")]
    public bool IsUpdatable { get; private set; }
}
