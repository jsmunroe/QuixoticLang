using Quixotic.Common.Tokens;

namespace Quixotic.Common.Expressions
{

    public abstract class QxExpression
    {
        public abstract ExpressionKind Kind { get; }

        public Span Span { get; set; } = Span.Empty;
    }
}
