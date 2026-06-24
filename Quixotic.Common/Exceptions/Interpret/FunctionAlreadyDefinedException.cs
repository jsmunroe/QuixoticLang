namespace Quixotic.Common.Exceptions.Interpret
{
    public class FunctionAlreadyDefinedException(string functionName) : Exception($"A function with the name '{functionName}' has already been defined.");
}