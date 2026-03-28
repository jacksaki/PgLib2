using PgLib2.Objects.Query;

namespace PgLib2.Objects;

public class PgTrigger : IPgObject
{
    public static SQLSet GetSQLSet() => PgTriggerQuery.GenerateSQLSet();
    internal PgTrigger(PgCatalog catalog)
    {
        _catalog = catalog;
    }
    public async Task<string> GenerateDDLAsync(DDLOptions options, CancellationToken ct = default)
    {
        ct.ThrowIfCancellationRequested();
        var sb = new System.Text.StringBuilder();
        sb.Append($"CREATE OR REPLACE TRIGGER ");
        if (options.AddSchema)
        {
            sb.Append($"{this.SchemaName}.");
        }
        sb.Append($"{this.Name} ");
        //
        //
        return await Task.FromResult(sb.ToString());
    }
    private PgCatalog _catalog;
    [DbColumn("trigger_schema")]
    public string SchemaName { get; private set; } = string.Empty;
    [DbColumn("trigger_name")]
    public string Name { get; private set; } = string.Empty;
    [DbColumn("event_manipulation")]
    public string? EventManipulation { get; private set; }
    [DbColumn("event_object_schema")]
    public string? EventObjectSchema { get; private set; }
    [DbColumn("event_object_table")]
    public string? EventObjectTableName { get; private set; }
    [DbColumn("event_object_table_oid")]
    public uint? EventObjectTableOid { get; private set; }

    [DbColumn("action_order")]
    public long? ActionOrder { get; private set; }
    [DbColumn("action_condition")]
    public string? ActionCondition { get; private set; }
    [DbColumn("action_statement")]
    public string? ActionStatement { get; private set; }
    [DbColumn("action_orientation")]
    public string? ActionOrientation { get; private set; }
    [DbColumn("action_timing")]
    public string? ActionTiming { get; private set; }
    [DbColumn("action_reference_old_table")]
    public string? ActionReferenceOldTable { get; private set; }
    [DbColumn("action_reference_new_table")]
    public string? ActionReferenceNewTable { get; private set; }
}
