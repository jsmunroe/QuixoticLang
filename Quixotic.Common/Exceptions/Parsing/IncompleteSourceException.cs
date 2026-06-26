using Quixotic.Common.Diagnostics;

namespace Quixotic.Common.Exceptions.Parsing
{
    public class IncompleteSourceException(string message, Diagnostic diagnostic) : ParserException(message, diagnostic)
    {
    }
}
