namespace Quixotic.Common.Exceptions.Interpret
{
    public class UnboundFunctionException(object function) : RuntimeException($"Function '{function}' must be bound to an instance before it is called.");
}
