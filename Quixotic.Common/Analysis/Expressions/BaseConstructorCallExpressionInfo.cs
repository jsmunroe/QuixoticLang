using Quixotic.Common.Expressions;
using Quixotic.Common.Symbols.Functions;
using Quixotic.Common.TypeSystem.Types;

namespace Quixotic.Common.Analysis.Expressions
{
    public class BaseConstructorCallExpressionInfo(QxType expressionType, QxExpression expression) : ExpressionInfo(expressionType, expression)
    {
        public required QxType BaseType { get; init; }
        public List<QxExpression> Arguments { get; init; } = []; // Arguments are not evaluated until the base type is called.
        public required BaseConstructor BaseConstructor { get; init; }

        protected override IEnumerable<AnalysisInfo> GetChildren()
        {
            return [];
        }
    }
}
