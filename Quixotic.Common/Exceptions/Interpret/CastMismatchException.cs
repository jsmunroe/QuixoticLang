using Quixotic.Common.Tokens;

namespace Quixotic.Common.Exceptions.Interpret
{
    public class CastMismatchException(object from, object to, Span span) : EnvironmentException($"Cannot cast from '{from}' to '{to}'.", span);
}
