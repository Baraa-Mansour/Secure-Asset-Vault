
namespace Core.Utilities.Attributes;

// this is just a marker atrribute
[AttributeUsage(AttributeTargets.Method)]
public class LogActionAttribute : Attribute
{
    public LogActionAttribute() { }
}
