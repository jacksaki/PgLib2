using System.Text.Json.Serialization;

namespace PgLib2.Schema;

public class PgPartitionTableChild
{
    [JsonPropertyName("oid")]
    [JsonInclude]
    private string _oid = string.Empty;

    [JsonIgnore]
    public uint Oid => _oid.ToUInt32();

    [JsonPropertyName("child_table_schema")]
    [JsonInclude]
    public string TableSchema { get; private set; } = string.Empty;

    [JsonPropertyName("child_table_name")]
    [JsonInclude]
    public string TableName { get; private set; } = string.Empty;
    [JsonPropertyName("partition_bound")]
    [JsonInclude]
    public string PartitionBound { get; private set; } = string.Empty;
}
