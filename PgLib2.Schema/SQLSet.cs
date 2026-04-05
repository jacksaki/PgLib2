using Npgsql;

namespace PgLib2.Schema;

public class SQLSet(string sql, NpgsqlParameter[]? parameters)
{
    public string SQL => sql;
    public NpgsqlParameter[]? Parameters => parameters;
    public NpgsqlParameter? this[string name]
        => this.Parameters?.FirstOrDefault(p => p.ParameterName.Equals(name, StringComparison.OrdinalIgnoreCase));

    public NpgsqlParameter? this[int index]
    {
        get
        {
            if (this.Parameters == null)
            {
                return null;
            }
            if (index < 0 || this.Parameters.Length <= index)
            {
                return null;
            }
            return this.Parameters[index];
        }
    }
}
