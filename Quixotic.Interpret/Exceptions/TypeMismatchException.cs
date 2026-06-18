namespace Quixotic.Interpret.Exceptions
{
    public class TypeMismatchException(Values.ValueType from, Values.ValueType to) : EnvironmentException($"Cannot convert from {from.Name} to {to.Name}.");
}
