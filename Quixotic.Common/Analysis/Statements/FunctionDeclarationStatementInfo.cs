using Quixotic.Common.Statements;
using Quixotic.Common.Symbols;
using Quixotic.Common.TypeSystem.Symbols;
using Quixotic.Common.TypeSystem.Types;

namespace Quixotic.Common.Analysis.Statements
{
    public class FunctionDeclarationStatementInfo(QxStatement statement) : StatementInfo(statement)
    {
        public required string Name { get; init; }
        public IReadOnlyList<Parameter> Parameters { get; init; } = [];
        public required QxType ReturnType { get; init; }

        public required SignatureSymbol SignatureSymbol { get; init; }
        public IReadOnlyCollection<StatementInfo> BodyStatements { get; set; } = [];

        protected override IEnumerable<AnalysisInfo> GetChildren()
        {
            return [.. BodyStatements];
        }
    }
}
