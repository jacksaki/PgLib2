namespace PgLib2;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public sealed class DbClassAttribute : Attribute
{
    public string PostProcessMethodName { get; }

    public DbClassAttribute(string postProcessMethodName)
    {
        PostProcessMethodName = postProcessMethodName;
    }
}