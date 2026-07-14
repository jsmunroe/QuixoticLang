namespace Quixotic.Common.Exceptions.Interpret
{
    public class FunctionBindTypeMismatchException(object from, object to) : RuntimeException($"Cannot bind a bindable function of '{from}' to '{to}'.");
}
