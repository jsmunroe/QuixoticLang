namespace Quixotic.Interpret.Exceptions
{
    public class UndefinedIdentifierException(string name) : RuntimeException($"No identifier named '{name}' has been defined.");
}
