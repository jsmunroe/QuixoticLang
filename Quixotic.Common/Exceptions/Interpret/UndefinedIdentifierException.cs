namespace Quixotic.Common.Exceptions.Interpret
{
    public class UndefinedIdentifierException(string name) : RuntimeException($"No identifier named '{name}' has been defined.");
}
