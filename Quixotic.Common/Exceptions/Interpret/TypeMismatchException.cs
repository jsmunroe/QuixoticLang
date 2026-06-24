namespace Quixotic.Common.Exceptions.Interpret
{
    public class TypeMismatchException(object from, object to) : EnvironmentException($"Cannot convert from {from} to {to}.");
}
