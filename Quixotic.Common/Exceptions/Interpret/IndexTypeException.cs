namespace Quixotic.Common.Exceptions.Interpret
{
    public class IndexTypeException(object arrayType, object indexType) : InterpreterException($"An {arrayType} cannot be indexed with a {indexType} value.");
}
