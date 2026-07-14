namespace Quixotic.Common.Exceptions.Interpret
{
    public class UndefinedPropertyException(object targetType, string propertyName) : RuntimeException($"Type '{targetType}' does not have a '{propertyName}' property.");
}
