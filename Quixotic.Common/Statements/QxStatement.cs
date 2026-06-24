using Quixotic.Common.Tokens;

namespace Quixotic.Common.Statements
{
    public abstract class QxStatement
    {
        public Span Span { get; set; } = Span.Empty;
    }
}
