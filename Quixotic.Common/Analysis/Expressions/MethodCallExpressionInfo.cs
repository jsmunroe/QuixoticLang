using Quixotic.Common.Expressions;
using Quixotic.Common.Symbols;
using Quixotic.Common.TypeSystem.Types;

namespace Quixotic.Common.Analysis.Expressions
{
    public class MethodCallExpressionInfo(QxType expressionType, QxExpression expression) : ExpressionInfo(expressionType, expression)
    {
        public required ExpressionInfo Target { get; init; }

        public required string MethodName { get; init; }

        public required CallType FunctionCallType { get; init; }

        public required bool IsInstanceCall { get; init; }
        public required bool IsStaticCall { get; init; }
        public required bool IsDynamic { get; set; }
        public required bool IsDeferred { get; set; }

        public IReadOnlyList<ExpressionInfo> Arguments { get; init; } = [];

        protected override IEnumerable<AnalysisInfo> GetChildren()
        {
            return [Target, .. Arguments];
        }

    }
}
