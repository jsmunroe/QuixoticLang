namespace Quixotic.Interpret.Exceptions
{
    public class UndefinedVariableException(string name) : EnvironmentException($"Variable '{name}' cannot be accessed because it is not defined.");
}
