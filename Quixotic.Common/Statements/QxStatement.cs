using Quixotic.Common.Analysis.Statements;
using Quixotic.Common.Tokens;

namespace Quixotic.Common.Statements
{
    public abstract class QxStatement
    {
        public Span Span { get; set; } = Span.Empty;

        public StatementInfo? Info { get; set; }
    }
}
