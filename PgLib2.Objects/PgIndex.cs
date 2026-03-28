using PgLib2.Objects.Query;
using System.Text.Json;
using ZLinq;

namespace PgLib2.Objects;

[DbClass(nameof(RefreshColumns))]
public class PgIndex : IPgObject
{
    public static SQLSet GetSQLSet() => PgIndexQuery.GenerateSQLSet();
    internal PgIndex(PgCatalog catalog)
    {
        _catalog = catalog;
    }
    private PgCatalog _catalog;

    internal void RefreshColumns()
    {
        if (!string.IsNullOrEmpty(_columns))
        {
            var columns = JsonSerializer.Deserialize<PgIndexColumn[]>(_columns);
            if (columns != null)
            {
                this.Columns = columns.ToList().AsReadOnly(); ;
            }
            else
            {
                this.Columns = Array.Empty<PgIndexColumn>().AsReadOnly();
            }
        }
        else
        {
            this.Columns = Array.Empty<PgIndexColumn>().AsReadOnly();
        }
    }

    public async Task<string> GenerateDDLAsync(DDLOptions options, CancellationToken ct = default)
    {
        ct.ThrowIfCancellationRequested();
        var sb = new System.Text.StringBuilder();
        sb.Append($"CREATE ");
        if (this.IsUnique)
        {
            sb.Append($"UNIQUE ");
        }
        sb.Append($"INDEX ");
        if (options.AddSchema)
        {
            sb.Append($"{this.SchemaName}.");
        }
        sb.Append($"{this.Name} ON ");
        if (options.AddSchema)
        {
            sb.Append($"{this.TableSchema}.");
        }
        sb.Append($"{this.TableName} ON (");
        sb.Append(this.Columns.AsValueEnumerable<PgIndexColumn>().Select(x => $"{x.ColumnName} {x.Order}").JoinToString(","));
        sb.Append($")");
        return await Task.FromResult(sb.ToString());
    }

    [DbColumn("index_oid")]
    public uint Oid { get; private set; }
    [DbColumn("table_oid")]
    public uint TableOid { get; private set; }
    [DbColumn("table_schema")]
    public string TableSchema { get; private set; } = string.Empty;
    [DbColumn("table_name")]
    public string TableName { get; private set; } = string.Empty;
    [DbColumn("index_schema")]
    public string SchemaName { get; private set; } = string.Empty;
    [DbColumn("index_name")]
    public string Name { get; private set; } = string.Empty;
    [DbColumn("is_primary_key")]
    public bool IsPrimaryKey { get; private set; }
    [DbColumn("is_unique")]
    public bool IsUnique { get; private set; }
    [DbColumn("definition")]
    private string Definition { get; set; } = string.Empty;
    [DbColumn("columns")]
    private string? _columns = null;
    public IReadOnlyList<PgIndexColumn> Columns { get; private set; } = Array.Empty<PgIndexColumn>().AsReadOnly();
}
