namespace PgLib2.Schema;

internal static class ConvertExtention
{
    public static uint ToUInt32(this object? value)
    {
        return value.ToUInt32(default);
    }
    public static uint ToUInt32(this object? value, uint defaultValue)
    {
        return value.ToUIntN() ?? defaultValue;
    }

    public static uint? ToUIntN(this object? value)
    {
        if (value == null || value == DBNull.Value)
        {
            return null;
        }
        return uint.TryParse(value.ToString(), out var ret) ? ret : null;
    }
}
