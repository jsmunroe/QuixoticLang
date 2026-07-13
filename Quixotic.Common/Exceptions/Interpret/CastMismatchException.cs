namespace Quixotic.Common.Exceptions.Interpret
{
    internal class CastMismatchException(object from, object to) : EnvironmentException($"Cannot cast from {from} to {to}.");
}
