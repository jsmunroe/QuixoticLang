namespace Quixotic.Interpret.Exceptions
{
    public class SymbolAlreadyDefinedException(string symbolName) : Exception($"A symbol with the name '{symbolName}' has already been defined.");
}