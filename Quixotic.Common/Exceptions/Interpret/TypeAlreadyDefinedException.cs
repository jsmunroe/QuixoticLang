namespace Quixotic.Common.Exceptions.Interpret
{
    public class TypeAlreadyDefinedException(string typeName) : Exception($"A type with the name '{typeName}' has already been defined.");
}