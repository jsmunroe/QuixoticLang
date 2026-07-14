using Quixotic.Common.Tokens;

namespace Quixotic.Common.Exceptions.Interpret
{
    public class InvalidAssignmentTargetException(object type, Span span) : InterpreterException($"Cannot assign to target of type {type}.", span);
}
