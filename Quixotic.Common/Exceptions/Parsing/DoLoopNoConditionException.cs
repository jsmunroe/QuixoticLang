using Quixotic.Common.Diagnostics;

namespace Quixotic.Common.Exceptions.Parsing
{
    public class DoLoopNoConditionException(Diagnostic diagnostic) : ParserException("Do loop lacks a condition.", diagnostic);
}
