using PgLib2.Schema.Query;

namespace PgLib2.Schema;

public class PgSequence : IPgObject
{
    public static SQLSet GetSQLSet() => PgSequenceQuery.GenerateSQLSet();

    public async Task<string> GenerateDDLAsync(DDLOptions options, CancellationToken ct = default)
    {
        ct.ThrowIfCancellationRequested();
        var sb = new System.Text.StringBuilder();
        sb.Append($"CREATE SEQUENCE ");
        if (options.AddSchema)
        {
            sb.Append($"{this.SchemaName}.");
        }
        sb.AppendLine($"{this.Name} ");
        sb.AppendLine($"INCREMENT BY {this.IncrementBy}");
        if (this.MinValue.HasValue)
        {
            sb.AppendLine($"NO MINVALUE");
        }
        else
        {
            sb.AppendLine($"MINVALUE {this.MinValue}");
        }
        if (this.MaxValue.HasValue)
        {
            sb.AppendLine($"NO MAXVALUE");
        }
        else
        {
            sb.AppendLine($"MAXVALUE {this.MaxValue}");
        }
        sb.AppendLine($"START WITH {this.StartValue}");
        sb.AppendLine($"START WITH {this.StartValue}");
        sb.AppendLine($"CACHE {this.CacheSize}");
        if (!this.IsCycled)
        {
            sb.Append($"NO ");
        }
        sb.AppendLine($"CYCLE");
        sb.Append($"OWNED BY ");
        if (this.OwnedTableName != null)
        {
            if (options.AddSchema)
            {
                sb.Append($"{this.OwnedTableSchema}.");
            }
            sb.AppendLine($"{this.OwnedTableName}.{this.OwnedColumn}");
        }
        else
        {
            sb.AppendLine($"OWNED BY NONE");
        }
        return await Task.FromResult(sb.ToString());
    }


    internal PgSequence(PgCatalog catalog)
    {
        _catalog = catalog;
    }
    private PgCatalog _catalog;
    [DbColumn("table_oid")]
    public uint TableOid { get; private set; }
    [DbColumn("sequence_schema")]
    public string SchemaName { get; private set; } = string.Empty;
    [DbColumn("sequence_name")]
    public string Name { get; private set; } = string.Empty;
    [DbColumn("data_type")]
    public string DataType { get; private set; } = string.Empty;
    [DbColumn("start_value")]
    public long StartValue { get; private set; }
    [DbColumn("increment_by")]
    public long IncrementBy { get; private set; }
    [DbColumn("min_value")]
    public long? MinValue { get; private set; }
    [DbColumn("max_value")]
    public long? MaxValue { get; private set; }
    [DbColumn("cache_size")]
    public long CacheSize { get; private set; }
    [DbColumn("is_cycled")]
    public bool IsCycled { get; private set; }

    [DbColumn("owned_table_schema")]
    public string? OwnedTableSchema { get; private set; }
    [DbColumn("owned_table_name")]
    public string? OwnedTableName { get; private set; }

    [DbColumn("owned_column")]
    public string? OwnedColumn { get; private set; }

}
