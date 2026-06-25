using Quixotic.Common.Statements;
using Quixotic.Common.Tokens;

namespace Quixotic.Parsing.Context
{
    public class StatementContext
    {
        public StatementType Type { get; set; }

        public QxStatement? Statement { get; set; }

        public List<Token> Tokens { get; } = [];

        public StatementContext? Parent { get; set; }

        public List<StatementContext> Children { get; } = [];

        public List<ExpressionContext> Expressions { get; } = [];

        public Span Span => Tokens.GetTotalSpan();
    }
}
