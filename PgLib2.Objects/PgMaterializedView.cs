using PgLib2.Objects.Query;
using ZLinq;
namespace PgLib2.Objects;

public class PgMaterializedView : PgRelationBase, IPgObject
{
    public static SQLSet GetSQLSet() => PgMaterializedViewQuery.GenerateSQLSet();

    public override async Task<string> GenerateDDLAsync(DDLOptions options, CancellationToken ct = default)
    {
        ct.ThrowIfCancellationRequested();
        var columns = await this.ListColumnsAsync(ct).ToListAsync(ct).ConfigureAwait(false);
        var sb = new System.Text.StringBuilder();
        sb.Append("CREATE OR REPLACE MATERIALIZED VIEW ");
        if (options.AddSchema)
        {
            sb.Append($"{this.SchemaName}.");
        }
        sb.AppendLine($"{this.Name} (");

        sb.AppendLine(columns.AsValueEnumerable<PgColumn>().OrderBy(x => x.OrdinalPosition).Select(x => x.GenerateColumnDDL()).JoinToString(",\n").Trim());
        sb.AppendLine(") AS");
        sb.AppendLine($"{this.ViewDefinition};");

        if (options.AddConstraints)
        {
            await foreach (var constraint in this.ListConstraintsAsync(ct).ConfigureAwait(false))
            {
                sb.AppendLine(await constraint.GenerateDDLAsync(options,ct).ConfigureAwait(false));
            }
        }
        if (options.AddIndexes)
        {
            await foreach (var index in this.ListIndexesAsync(ct).ConfigureAwait(false))
            {
                if (!options.AddConstraints || (!index.IsPrimaryKey && !index.IsUnique))
                {
                    sb.AppendLine(await index.GenerateDDLAsync(options, ct).ConfigureAwait(false));
                }
            }
        }
        return sb.ToString();
    }

    internal PgMaterializedView(PgCatalog catalog) : base(catalog)
    {
    }

    [DbColumn("oid")]
    public uint Oid
    {
        get => base._oid;
        private set => base._oid = value;
    }

    [DbColumn("comment")]
    public override string? Comment { get; protected set; }

    [DbColumn("mview_schema")]
    public override string SchemaName { get; protected set; } = string.Empty;
    [DbColumn("mview_name")]
    public override string Name { get; protected set; } = string.Empty;

    [DbColumn("view_definition")]
    public string? ViewDefinition { get; private set; }
    [DbColumn("is_insertable_into")]
    public bool CanInsert { get; private set; }
}
