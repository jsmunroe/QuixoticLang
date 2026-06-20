namespace Quixotic.Interpret.Exceptions
{
    public class UndefinedSymbolException(string name) : RuntimeException($"No symbol named '{name}' has been defined.");
}
