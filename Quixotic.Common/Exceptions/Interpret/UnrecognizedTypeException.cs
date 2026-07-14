using Quixotic.Common.Tokens;

namespace Quixotic.Common.Exceptions.Interpret
{
    public class UnrecognizedTypeException(object type, Span span) : EnvironmentException($"The type '{type}' is not recognized.", span);
}
