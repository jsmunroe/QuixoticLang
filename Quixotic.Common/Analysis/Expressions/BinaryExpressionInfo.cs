using Quixotic.Common.Expressions;
using Quixotic.Common.Operations;
using Quixotic.Common.Symbols;
using Quixotic.Common.TypeSystem.Types;

namespace Quixotic.Common.Analysis.Expressions
{
    public class BinaryExpressionInfo(QxType expressionType, QxExpression expression) : ExpressionInfo(expressionType, expression)
    {
        public required Operator Operator { get; init; }
        public required ExpressionInfo Left { get; init; }
        public required ExpressionInfo Right { get; init; }

        public required SignatureSymbol SignatureSymbol { get; init; }


        protected override IEnumerable<AnalysisInfo> GetChildren()
        {
            return [Left, Right];
        }

    }
}
