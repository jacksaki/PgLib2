using PgLib2.Objects;
using ZLinq;
namespace PgLib2.DefinitionReport;

public class DefinitionReportGenerator
{
    private PgCatalog _catalog;
    private string _schemaName;
    private string _relationName;
    private DefinitionReportGenerator(PgCatalog catalog, string schemaName, string relationName)
    {
        _catalog = catalog;
        _schemaName = schemaName;
        _relationName = relationName;
    }
    private PgRelationBase? _relation;

    public static async Task<DefinitionReportGenerator> CreateAsync(PgCatalog catalog, string schemaName, string relationName, CancellationToken ct = default)
    {
        var report = new DefinitionReportGenerator(catalog, schemaName, relationName);
        var rels = typeof(PgRelationBase).Assembly.GetTypes().AsValueEnumerable().Where(t => t.IsSubclassOf(typeof(PgRelationBase)));
        foreach (var rel in rels)
        {
            var obj = await catalog.GetAsync(rel, schemaName, relationName, ct) as PgRelationBase;
            if (obj != null)
            {
                report._relation = obj;
                return report;
            }
        }
        throw new Exception($"Relation {schemaName}.{relationName} not found in catalog.");
    }

    public async Task<string> GeneratePlainTextReportAsync(CancellationToken cancellationToken = default)
    {

        return string.Empty;
    }
    public async Task<string> GenerateMarkdownReportAsync(CancellationToken cancellationToken = default)
    {
        return string.Empty;
    }
    public async Task<string> GenerateExcelReportAsync(ExcelReportConfig conf, CancellationToken cancellationToken = default)
    {
        return string.Empty;
    }
}

