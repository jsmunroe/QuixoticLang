using Quixotic.Common.Tokens;

namespace Quixotic.Common.Expressions
{

    public abstract class QxExpression
    {
        public Span Span { get; set; } = Span.Empty;

        public SemanticInfo? SemanticInfo { get; set; }
    }
}
