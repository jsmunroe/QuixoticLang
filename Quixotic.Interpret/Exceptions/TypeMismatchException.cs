namespace Quixotic.Interpret.Exceptions
{
    public class TypeMismatchException(Symbols.QxType from, Symbols.QxType to) : EnvironmentException($"Cannot convert from {from.Name} to {to.Name}.");
}
