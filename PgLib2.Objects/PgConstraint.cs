using PgLib2.Objects.Query;
using System.Text.Json;
using ZLinq;
namespace PgLib2.Objects;

[DbClass(nameof(RefreshColumns))]
public class PgConstraint : IPgObject
{
    public static SQLSet GetSQLSet() => PgConstraintQuery.GenerateSQLSet();
    internal PgConstraint(PgCatalog catalog)
    {
        _catalog = catalog;
    }

    private PgCatalog _catalog;
    private void RefreshColumns()
    {
        if (!string.IsNullOrEmpty(_columns))
        {
            var array = JsonSerializer.Deserialize<PgConstraintColumn[]>(_columns);
            if (array == null)
            {
                this.ColumnNames = Array.Empty<string>().AsReadOnly();
            }
            else
            {
                this.ColumnNames = array.AsValueEnumerable<PgConstraintColumn>().Select(x => x.ColumnName).ToList().AsReadOnly();
            }
        }
        else
        {
            this.ColumnNames = Array.Empty<string>().AsReadOnly();
        }
    }

    public async Task<string> GenerateDDLAsync(DDLOptions options, CancellationToken ct = default)
    {
        ct.ThrowIfCancellationRequested();
        var sb = new System.Text.StringBuilder();
        sb.Append("ALTER TABLE ");
        if (options.AddSchema)
        {
            sb.Append($"{this.TableSchema}.");
        }
        sb.Append($"{this.TableName} ADD CONSTRAINT ");
        sb.Append($"{this.Definition};");
        return await Task.FromResult(sb.ToString());
    }

    [DbColumn("table_oid")]
    public uint TableOid { get; private set; }
    [DbColumn("table_schema")]
    public string TableSchema { get; private set; } = string.Empty;
    [DbColumn("table_name")]
    public string TableName { get; private set; } = string.Empty;
    [DbColumn("constraint_schema")]
    public string SchemaName { get; private set; } = string.Empty;
    [DbColumn("constraint_name")]
    public string Name { get; private set; } = string.Empty;
    [DbColumn("constraint_type")]
    public string? ConstraintType { get; private set; }

    [DbColumn("columns")]
    private string? _columns = null;

    public IReadOnlyList<string> ColumnNames { get; private set; } = Array.Empty<string>();

    [DbColumn("definition")]
    public string Definition { get; private set; } = string.Empty;

    [DbColumn("foreign_table_schema")]
    public string? ForeignTableSchema { get; private set; }
    [DbColumn("foreign_table_name")]
    public string? ForeignTableName { get; private set; }

}
