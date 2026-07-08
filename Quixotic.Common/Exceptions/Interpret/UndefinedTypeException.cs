namespace Quixotic.Common.Exceptions.Interpret
{
    public class UndefinedTypeException(string name) : RuntimeException($"No type named '{name}' has been defined.");
}
