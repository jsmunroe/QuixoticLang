namespace Quixotic.Interpret.Exceptions
{
    public class VariableAlreadyDefinedException(string variableName) : Exception($"A variable with the name '{variableName}' has already been defined.");
}