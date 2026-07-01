namespace Quixotic.Common.Exceptions.Interpret
{
    public class IndexerTargetException(object target) : InterpreterException($"An instance of type {target} cannot be indexed.");
}
