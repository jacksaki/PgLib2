using PgLib2.Objects.Query;
using ZLinq;
namespace PgLib2.Objects;
public class DDLOptions
{
    public bool AddConstraints { get; set; }
    public bool AddIndexes { get; set; }
    public bool AddSchema { get; set; }
}

public sealed class PgTable : PgRelationBase, IPgObject
{
    public static SQLSet GetSQLSet() => PgTableQuery.GenerateSQLSet();

    public override async Task<string> GenerateDDLAsync(DDLOptions options, CancellationToken ct = default)
    {
        ct.ThrowIfCancellationRequested();
        var columns = await this.ListColumnsAsync().ToListAsync().ConfigureAwait(false);
        var sb = new System.Text.StringBuilder();
        sb.Append("CREATE TABLE ");
        if (options.AddSchema)
        {
            sb.Append($"{this.SchemaName}.");
        }
        sb.AppendLine($"{this.Name} (");
        sb.AppendLine(columns.AsValueEnumerable<PgColumn>().OrderBy(x => x.OrdinalPosition).Select(x => x.GenerateColumnDDL()).JoinToString(",\n").Trim());
        sb.AppendLine(");");
        if (options.AddConstraints)
        {
            await foreach (var constraint in this.ListConstraintsAsync())
            {
                sb.AppendLine(await constraint.GenerateDDLAsync(options));
            }
        }
        if (options.AddIndexes)
        {
            await foreach (var index in this.ListIndexesAsync())
            {
                if (options.AddConstraints)
                {
                    if (index.IsPrimaryKey || index.IsUnique)
                    {
                        continue;
                    }
                }
                sb.AppendLine(await index.GenerateDDLAsync(options));
            }
        }
        return sb.ToString();
    }

    internal PgTable(PgCatalog catalog) : base(catalog)
    {
    }

    [DbColumn("oid")]
    public uint Oid
    {
        get => base._oid;
        private set => base._oid = value;
    }

    [DbColumn("table_schema")]
    public string SchemaName { get; private set; } = string.Empty;

    [DbColumn("table_name")]
    public string Name { get; private set; } = string.Empty;

    [DbColumn("is_insertable_into")]
    public bool CanInsert { get; private set; }
}
