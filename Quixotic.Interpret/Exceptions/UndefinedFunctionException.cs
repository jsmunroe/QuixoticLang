namespace Quixotic.Interpret.Exceptions
{
    public class UndefinedFunctionException(string name) : RuntimeException($"No function named '{name}' has been defined.");
}
