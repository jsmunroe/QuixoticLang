using Quixotic.Common.Contracts;
using Quixotic.Common.Diagnostics;

namespace Quixotic.Common.Exceptions.Parsing
{
    public class ParserException(string message, Diagnostic diagnostic) : Exception(message), IHasDiagnostic
    {
        public Diagnostic Diagnostic { get; } = diagnostic;
    }
}
