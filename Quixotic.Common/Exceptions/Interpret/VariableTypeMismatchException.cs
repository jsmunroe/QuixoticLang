namespace Quixotic.Common.Exceptions.Interpret
{
    public class VariableTypeMismatchException(object from, object to) : RuntimeException($"Cannot assign a variable of type {from} with a value of type {to}");
}
