using Quixotic.Analysis.Semantics;
using Quixotic.Common.Tokens;

namespace Quixotic.Analysis.Exceptions
{
    public class AssignmentTypeMismatchException : SemanticException
    {
        public AssignmentTypeMismatchException(string name, object assigneeType, object assignedType, Span span) : base($"Variable {name} of type {assigneeType} cannot be assigned a {assignedType}.", span, Severity.Error)
        { }

        public AssignmentTypeMismatchException(object assigneeType, object assignedType, Span span) : base($"Cannot assign a {assignedType} to a variable of type {assigneeType}.", span, Severity.Error)
        { }
    }
}
