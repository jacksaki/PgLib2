using PgLib2.Objects.Query;
using ZLinq;
namespace PgLib2.Objects;

public class PgView : PgRelationBase, IPgObject
{
    public static SQLSet GetSQLSet() => PgViewQuery.GenerateSQLSet();
    internal PgView(PgCatalog catalog) : base(catalog)
    {
    }
    public override async Task<string> GenerateDDLAsync(DDLOptions options, CancellationToken ct = default)
    {
        var columns = await this.ListColumnsAsync(ct).ToListAsync(ct).ConfigureAwait(false);
        var sb = new System.Text.StringBuilder();
        sb.AppendLine($"CREATE OR REPLACE VIEW {this.SchemaName}.{this.Name} (");
        sb.AppendLine(columns.AsValueEnumerable<PgColumn>().OrderBy(x => x.OrdinalPosition).Select(x => x.ColumnName).JoinToString(",\n").Trim());
        sb.AppendLine(") AS");
        sb.AppendLine(this.ViewDefinition);
        return sb.ToString();
    }

    [DbColumn("oid")]
    public uint Oid
    {
        get => base._oid;
        private set => base._oid = value;
    }

    [DbColumn("view_schema")]
    public override string SchemaName { get; protected set; } = string.Empty;
    [DbColumn("view_name")]
    public override string Name { get; protected set; } = string.Empty;
    [DbColumn("comment")]
    public override string? Comment { get; protected set; }
    [DbColumn("view_definition")]
    public string? ViewDefinition { get; private set; } = string.Empty;
    [DbColumn("is_insertable_into")]
    public bool CanInsert { get; private set; }
}
