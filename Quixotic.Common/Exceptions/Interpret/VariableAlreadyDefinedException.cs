namespace Quixotic.Common.Exceptions.Interpret
{
    public class VariableAlreadyDefinedException(string variableName) : Exception($"A variable with the name '{variableName}' has already been defined.");
}