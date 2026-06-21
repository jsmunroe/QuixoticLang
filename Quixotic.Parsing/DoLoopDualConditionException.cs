using Quixotic.Parsing.Exceptions;

namespace Quixotic.Parsing
{
    public class DoLoopDualConditionException() : ParserException("Do loop has both an entry- and an exit-control condition.");

    public class DoLoopNoConditionException() : ParserException("Do loop lacks a condition.");
}
