using Quixotic.Common.Analysis.Expressions;
using Quixotic.Common.Symbols;
using Quixotic.Common.TypeSystem.Symbols;
using Quixotic.Common.TypeSystem.Types;

namespace Quixotic.Common.Analysis.Statements
{
    public class ConstructorDeclarationStatementInfo : StatementInfo
    {
        public required QxType Type { get; init; }
        public required IReadOnlyList<Parameter> Parameters { get; init; }
        public required ExpressionInfo? BaseCall { get; init; }
        public required SignatureSymbol? SignatureSymbol { get; init; }
    }
}
