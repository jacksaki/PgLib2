using PgLib2.Objects.Query;

namespace PgLib2.Objects;

public class PgProcedure : IPgObject
{
    public static SQLSet GetSQLSet() => PgProcedureQuery.GenerateSQLSet();
    public async Task<string> GenerateDDLAsync(DDLOptions options, CancellationToken ct = default)
    {
        ct.ThrowIfCancellationRequested();
        var sb = new System.Text.StringBuilder();
        sb.Append($"CREATE OR REPLACE PROCEDURE ");
        if (options.AddSchema)
        {
            sb.Append($"{this.SchemaName}.");
        }
        sb.AppendLine($"{this.Name} ");

        sb.AppendLine($"{this.Definition}");

        return await Task.FromResult(sb.ToString());
    }

    internal PgProcedure(PgCatalog catalog)
    {
        _catalog = catalog;
    }
    private PgCatalog _catalog;

    [DbColumn("routine_schema")]
    public string SchemaName { get; private set; } = string.Empty;
    [DbColumn("routine_name")]
    public string Name { get; private set; } = string.Empty;
    [DbColumn("data_type")]
    public string? DataType { get; private set; }
    [DbColumn("routine_body")]
    public string? Body { get; private set; }
    [DbColumn("routine_definition")]
    public string? Definition { get; private set; }
    [DbColumn("external_name")]
    public string? ExternalName { get; private set; }
    [DbColumn("external_language")]
    public string? ExternalLanguage { get; private set; }
    [DbColumn("is_deterministic")]
    public bool IsDeterministic { get; private set; }
}
