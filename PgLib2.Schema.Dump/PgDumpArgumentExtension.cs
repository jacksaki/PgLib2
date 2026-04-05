using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using ZLinq;

namespace PgLib2.Schema.Dump;

internal static class PgDumpArgumentExtension
{
    public static string? ToArgument(this PropertyInfo p, PgDumpArgumentAttribute attr, object value)
    {
        if (p.PropertyType == typeof(bool))
        {
            var val = (bool)value;
            if (val == true)
            {
                return attr.ArgumentName;
            }
        }
        else if (p.PropertyType == typeof(string))
        {
            var val = (string)value;
            if (!string.IsNullOrEmpty(val))
            {
                if (attr.AddEqual)
                {
                    return $"{attr.ArgumentName}=\"{val}\"";
                }
                else
                {
                    return $"{attr.ArgumentName} \"{val}\"";
                }
            }
        }
        return null;
    }

    public static string ToArgument(this DumpSettings d)
    {
        var props = d.GetType().GetProperties().
            AsValueEnumerable().
            Where(x => x.GetCustomAttribute<PgDumpArgumentAttribute>() != null).
            Select(x => new { Property = x, Attribute = x.GetCustomAttribute<PgDumpArgumentAttribute>()!, Value = x.GetValue(d)! });

        return props.Select(x => x.Property.ToArgument(x.Attribute, x.Value)).Where(arg => arg != null).JoinToString(" ");
    }
}