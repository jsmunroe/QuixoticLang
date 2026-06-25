using Quixotic.Common.Expressions;
using Quixotic.Common.Tokens;

namespace Quixotic.Parsing.Context
{
    public class ExpressionContext
    {
        public QxExpression? Expression { get; set; }

        public List<Token> Tokens { get; } = [];

        public ExpressionContext? Parent { get; set; }

        public List<ExpressionContext> Children { get; } = [];

        public Span Span => Tokens.GetTotalSpan();
    }
}
