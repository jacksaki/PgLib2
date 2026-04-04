using System.Buffers.Text;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using ZLinq;

namespace YamaKit.Text;

public static class ConvertExtensions
{
    public static string[] YesValues = new string[] { "Y", "YES", "T", "TRUE" };
    public static bool? ToBoolN(this object? value)
    {
        if (value == null || value == DBNull.Value)
        {
            return null;
        }
        return YesValues.AsValueEnumerable<string>().Where(x => x.Equals(value.ToString(), StringComparison.OrdinalIgnoreCase)).Any();
    }
    public static bool ToBool(this object? value, bool defaultValue)
    {
        return value.ToBoolN() ?? defaultValue;
    }
    public static sbyte? ToSByteN(this object? value)
    {
        if (value == null || value == DBNull.Value)
        {
            return null;
        }
        return sbyte.TryParse(value.ToString(), out var ret) ? ret : null;
    }
    public static sbyte ToSByte(this object? value, sbyte defaultValue)
    {
        return value.ToSByteN() ?? defaultValue;
    }

    public static byte? ToByteN(this object? value)
    {
        if (value == null || value == DBNull.Value)
        {
            return null;
        }
        return byte.TryParse(value.ToString(), out var ret) ? ret : null;
    }
    public static byte ToByte(this object? value, byte defaultValue)
    {
        return value.ToByteN() ?? defaultValue;
    }
    public static short? ToShortN(this object? value)
    {
        if (value == null || value == DBNull.Value)
        {
            return null;
        }
        return short.TryParse(value.ToString(), out var ret) ? ret : null;
    }
    public static short ToShort(this object? value, short defaultValue)
    {
        return value.ToShortN() ?? defaultValue;
    }
    public static ushort? ToUShortN(this object? value)
    {
        if (value == null || value == DBNull.Value)
        {
            return null;
        }
        return ushort.TryParse(value.ToString(), out var ret) ? ret : null;
    }
    public static ushort ToUShort(this object? value, ushort defaultValue)
    {
        return value.ToUShortN() ?? defaultValue;
    }
    public static int? ToIntN(this object? value)
    {
        if (value == null || value == DBNull.Value)
        {
            return null;
        }
        return int.TryParse(value.ToString(), out var ret) ? ret : null;
    }
    public static int ToInt32(this object? value, int defaultValue)
    {
        return value.ToIntN() ?? defaultValue;
    }
    public static uint? ToUIntN(this object? value)
    {
        if (value == null || value == DBNull.Value)
        {
            return null;
        }
        return uint.TryParse(value.ToString(), out var ret) ? ret : null;
    }
    public static uint ToUInt32(this object? value, uint defaultValue)
    {
        return value.ToUIntN() ?? defaultValue;
    }
    public static long? ToLongN(this object? value)
    {
        if (value == null || value == DBNull.Value)
        {
            return null;
        }
        return long.TryParse(value.ToString(), out var ret) ? ret : null;
    }
    public static long ToLong(this object? value, long defaultValue)
    {
        return value.ToLongN() ?? defaultValue;
    }
    public static ulong? ToULongN(this object? value)
    {
        if (value == null || value == DBNull.Value)
        {
            return null;
        }
        return ulong.TryParse(value.ToString(), out var ret) ? ret : null;
    }
    public static ulong ToULong(this object? value, ulong defaultValue)
    {
        return value.ToULongN() ?? defaultValue;
    }
    public static float? ToFloatN(this object? value)
    {
        if (value == null || value == DBNull.Value)
        {
            return null;
        }
        return float.TryParse(value.ToString(), out var ret) ? ret : null;
    }
    public static float ToFloat(this object? value, float defaultValue)
    {
        return value.ToFloatN() ?? defaultValue;
    }
    public static double? ToDoubleN(this object? value)
    {
        if (value == null || value == DBNull.Value)
        {
            return null;
        }
        return double.TryParse(value.ToString(), out var ret) ? ret : null;
    }
    public static double ToDouble(this object? value, double defaultValue)
    {
        return value.ToDoubleN() ?? defaultValue;
    }
    public static decimal? ToDecimalN(this object? value)
    {
        if (value == null || value == DBNull.Value)
        {
            return null;
        }
        return decimal.TryParse(value.ToString(), out var ret) ? ret : null;
    }
    public static decimal ToDecimal(this object? value, decimal defaultValue)
    {
        return value.ToDecimalN() ?? defaultValue;
    }

    public static DateTime? ToDateTimeN(this object? value)
    {
        if (value == null || value == DBNull.Value)
        {
            return null;
        }
        if (value is DateTime dt)
        {
            return dt;
        }
        else if (value is DateOnly d)
        {
            return d.ToDateTime(TimeOnly.MinValue);
        }
        else
        {
            var dd = value.ToDoubleN();
            if (dd.HasValue)
            {
                return DateTime.FromOADate(dd.Value);
            }
            else
            {
                return null;
            }
        }
    }
    public static DateTime ToDateTime(this object? value, DateTime defaultValue)
    {
        return value.ToDateTimeN() ?? defaultValue;
    }

    public static DateTime? ToDateTimeN(this object? value, string dateFormat)
    {
        if (value == null || value == DBNull.Value)
        {
            return null;
        }
        return DateTime.TryParseExact(value.ToString(), dateFormat, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out var dt) ? dt : null;
    }
    public static DateTime ToDateTime(this object? value, string dateFormat, DateTime defaultValue)
    {
        return value.ToDateTimeN(dateFormat) ?? defaultValue;
    }
    public static string ToSnakeCase(this string text)
    {
        if (text == null)
        {
            throw new ArgumentNullException(nameof(text));
        }
        if (text.Length < 2)
        {
            return text;
        }
        var sb = new StringBuilder();
        sb.Append(char.ToLowerInvariant(text[0]));
        for (int i = 1; i < text.Length; ++i)
        {
            char c = text[i];
            if (char.IsUpper(c))
            {
                sb.Append('_');
                sb.Append(char.ToLowerInvariant(c));
            }
            else
            {
                sb.Append(c);
            }
        }
        return sb.ToString();
    }
    public static string ToSha256(this string value)
    {
        return System.Text.Encoding.UTF8.GetBytes(value).ToSha256();
    }

    public static string ToSha256(this byte[] value)
    {
        var sha256 = SHA256.Create();

        var bytes = sha256.ComputeHash(value);
        sha256.Clear();
        return string.Join("", bytes.Select(x => $"{x:x2}"));
    }

    public static string UrlEncode(this string value)
    {
        return System.Web.HttpUtility.UrlEncode(value);
    }

    public static string UrlDeocde(this string value)
    {
        return System.Web.HttpUtility.UrlDecode(value);
    }

    public static string Base64Encode(this string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return string.Empty;
        }
        return Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(value));
    }

    public static byte[] Base64Decode(this string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return new byte[] { };
        }
        var buffer = new byte[value.Length]; // 適当に確保
        if (Convert.TryFromBase64String(value, buffer, out var written))
        {
            return buffer.AsSpan(0, written).ToArray();
        }
        else
        {
            return Array.Empty<byte>();
        }
    }

    public static string ToPascalCase(this string original)
    {
        var invalidCharsRgx = new Regex("[^_a-zA-Z0-9]");
        var whiteSpace = new Regex(@"(?<=\s)");
        var startsWithLowerCaseChar = new Regex("^[a-z]");
        var firstCharFollowedByUpperCasesOnly = new Regex("(?<=[A-Z])[A-Z0-9]+$");
        var lowerCaseNextToNumber = new Regex("(?<=[0-9])[a-z]");
        var upperCaseInside = new Regex("(?<=[A-Z])[A-Z]+?((?=[A-Z][a-z])|(?=[0-9]))");

        // replace white spaces with undescore, then replace all invalid chars with empty string
        var pascalCase = invalidCharsRgx.Replace(whiteSpace.Replace(original, "_"), string.Empty)
            .Split(new char[] { '_' }, StringSplitOptions.RemoveEmptyEntries).AsValueEnumerable()
            .Select(w => startsWithLowerCaseChar.Replace(w, m => m.Value.ToUpper()))
            .Select(w => firstCharFollowedByUpperCasesOnly.Replace(w, m => m.Value.ToLower()))
            .Select(w => lowerCaseNextToNumber.Replace(w, m => m.Value.ToUpper()))
            .Select(w => upperCaseInside.Replace(w, m => m.Value.ToLower()));

        return pascalCase.JoinToString("");
    }
}
