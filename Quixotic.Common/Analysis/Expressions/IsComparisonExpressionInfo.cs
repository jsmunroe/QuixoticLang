using Quixotic.Common.Expressions;
using Quixotic.Common.Symbols;
using Quixotic.Common.TypeSystem.Types;
using System.Diagnostics.CodeAnalysis;

namespace Quixotic.Common.Analysis.Expressions
{
    public class IsComparisonExpressionInfo(QxExpression expression) : ExpressionInfo(QxType.Boolean, expression)
    {
        public required ExpressionInfo Target { get; init; }

        public required QxType Type { get; init; }

        [MemberNotNullWhen(true, nameof(PatternIdentifier))]
        public required bool Result { get; init; }

        public required IdentifierSymbol? PatternIdentifier { get; init; }
    }
}
