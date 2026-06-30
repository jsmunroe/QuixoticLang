namespace Quixotic.Common.Exceptions.Interpret
{
    public class InvalidAssignmentTargetException(object type) : InterpreterException($"Cannot assign to target of type {type}.");
}
