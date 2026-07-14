using Quixotic.Common.Tokens;

namespace Quixotic.Common.Exceptions.Interpret
{
    public class IndexTypeException(object arrayType, object indexType, Span span) : InterpreterException($"An {arrayType} cannot be indexed with a {indexType} value.", span);
}
