using Quixotic.Common.Analysis.Expressions;
using Quixotic.Common.Statements;
using Quixotic.Common.Symbols;
using Quixotic.Common.TypeSystem.Symbols;
using Quixotic.Common.TypeSystem.Types;

namespace Quixotic.Common.Analysis.Statements
{
    public class ConstructorDeclarationStatementInfo(QxStatement statement) : StatementInfo(statement)
    {
        public required QxType Type { get; init; }
        public required IReadOnlyList<Parameter> Parameters { get; init; }
        public required ExpressionInfo? BaseCall { get; init; }
        public required SignatureSymbol? SignatureSymbol { get; init; }

        protected override IEnumerable<AnalysisInfo?> GetChildren()
        {
            return [BaseCall];
        }
    }
}
