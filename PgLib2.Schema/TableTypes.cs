namespace PgLib2.Schema;

[Flags]
public enum TableTypes
{
    [TableType("BASE TABLE", "r")]
    BaseTable = 1,
    [TableType("VIEW", "r")]
    View = 2,
    [TableType("PARTITION TABLE", "p")]
    PartitionTable = 4,
    [TableType("MATERIALIZED VIEW", "m")]
    MaterializedView = 8,
    [TableType("TOAST TABLE", "t")]
    ToastTable = 16,
    [TableType("FOREIGN TABLE", "f")]
    ForeignTable = 32,
}
