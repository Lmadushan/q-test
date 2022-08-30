using System.Reflection;
namespace QualitAppsTest.Common.Attribute;

public class SqlOutputDirectionAttribute : System.Attribute
{

    public bool IsOutputDirection { get; set; }

    public SqlOutputDirectionAttribute(bool value)
    {
        IsOutputDirection = value;
    }
}
public abstract class IsJsonStringAttribute : System.Attribute
{

    public bool IsOutputDirection { get; set; }
    public bool IsJSON { get; set; }

    protected IsJsonStringAttribute(bool value)
    {
        IsJSON = value;
    }
}
public static class SqlOutputDirectionHelper
{
    public static bool IsOutputDirection<T>(this T obj, PropertyInfo p)
    {
        object[] attrs = p.GetCustomAttributes(true);
        bool result = false;
        foreach (object attr in attrs)
        {
            if (attr is SqlOutputDirectionAttribute attribute)
            {
                result = attribute.IsOutputDirection;
            }
        }
        return result;
    }

    public static bool IsJSON<T>(this T obj, PropertyInfo p)
    {
        object[] attrs = p.GetCustomAttributes(true);
        bool result = false;
        foreach (object attr in attrs)
        {
            if (attr is IsJsonStringAttribute attribute)
            {
                result = attribute.IsJSON;
            }
        }
        return result;
    }
}
