using Quixotic.Common.Diagnostics;

namespace Quixotic.Common.Exceptions.Parsing
{
    public class DoLoopDualConditionException(Diagnostic diagnostic) : ParserException("Do loop has both an entry- and an exit-control condition.", diagnostic);
}
