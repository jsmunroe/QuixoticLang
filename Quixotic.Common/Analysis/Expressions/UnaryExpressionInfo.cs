using Quixotic.Common.Expressions;
using Quixotic.Common.Operations;
using Quixotic.Common.Symbols;
using Quixotic.Common.TypeSystem.Types;

namespace Quixotic.Common.Analysis.Expressions
{
    public class UnaryExpressionInfo(QxType expressionType, QxExpression expression) : ExpressionInfo(expressionType, expression)
    {
        public required Operator Operator { get; init; }
        public required ExpressionInfo Operand { get; init; }

        public required SignatureSymbol SignatureSymbol { get; init; }
    }
}
