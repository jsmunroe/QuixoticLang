namespace Quixotic.Common.Exceptions.Interpret
{
    public class UndefinedConstructorException(object targetType) : RuntimeException($"Type '{targetType}' does not have a constructor that accepts the provided argument types.");
}
