namespace Quixotic.Interpret.Exceptions
{
    public class TypeMismatchException(Symbols.ValueType from, Symbols.ValueType to) : EnvironmentException($"Cannot convert from {from.Name} to {to.Name}.");
}
