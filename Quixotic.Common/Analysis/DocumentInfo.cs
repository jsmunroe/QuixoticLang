using Quixotic.Common.Analysis.Statements;
using Quixotic.Common.Tokens;

namespace Quixotic.Common.Analysis
{
    public class DocumentInfo : AnalysisInfo
    {
        public IReadOnlyList<StatementInfo> RootStatements { get; }

        public string? DocumentUri { get; set; }

        public DocumentInfo(IEnumerable<StatementInfo> rootStatements) : base(Span.Empty)
        {
            RootStatements = [.. rootStatements];

            ConnectParents();
        }

        protected override IEnumerable<AnalysisInfo?> GetChildren()
        {
            return [.. RootStatements];
        }
    }
}
