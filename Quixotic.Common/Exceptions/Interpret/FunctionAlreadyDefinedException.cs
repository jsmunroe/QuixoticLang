namespace Quixotic.Common.Exceptions.Interpret
{
    public class FunctionAlreadyDefinedException(object functionName) : Exception($"A function with the name '{functionName}' has already been defined.");
}