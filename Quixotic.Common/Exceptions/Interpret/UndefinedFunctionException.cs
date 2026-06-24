namespace Quixotic.Common.Exceptions.Interpret
{
    public class UndefinedFunctionException(string name) : RuntimeException($"No function named '{name}' has been defined.");
}
