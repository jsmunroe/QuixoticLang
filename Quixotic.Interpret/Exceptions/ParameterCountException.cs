namespace Quixotic.Interpret.Exceptions
{
    public class ParameterCountException(string functionName, int expected, int actual) : Exception($"The function named '{functionName}' requires {expected} parameters. {actual} arguments were provided.");
}
