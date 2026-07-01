using Quixotic.Common.Diagnostics;

namespace Quixotic.Common.Exceptions.Parsing
{
    public class BinaryExpressionException(object left, string @operator, object right, Diagnostic diagnostic) : ParserException($"{@operator} is not supported between values of type {left} and {right}.", diagnostic)
    {
    }
}
