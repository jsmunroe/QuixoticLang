using Quixotic.Common.Tokens;

namespace Quixotic.Common.Exceptions.Interpret
{
    public class IndexerTargetException(object target, Span span) : InterpreterException($"An instance of type {target} cannot be indexed.", span);
}
