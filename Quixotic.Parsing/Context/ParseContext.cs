using Quixotic.Common.Contracts;
using Quixotic.Common.Expressions;
using Quixotic.Common.Source;
using Quixotic.Common.Statements;
using Quixotic.Common.Tokens;

namespace Quixotic.Parsing.Context
{
    public class ParseContext(ISource source) : IParseContext
    {
        public SourceDocument _document = new(source);
        public List<StatementContext> _statements = [];

        public StatementContext? CurrentStatement { get; private set; }

        public ExpressionContext? CurrentExpression { get; private set; }

        public void BeginStatement()
        {
            var newStatement = new StatementContext();

            if (CurrentStatement is null)
                _statements.Add(newStatement);

            CurrentStatement?.Children.Add(newStatement);
            newStatement.Parent = CurrentStatement;

            CurrentStatement = newStatement;
        }

        public void AttachStatement(QxStatement statement)
        {
            if (CurrentStatement is not null)
            {
                statement.Span = CurrentStatement.Span;
                CurrentStatement.Statement = statement;
            }
        }

        public void EndStatement()
        {
            CurrentStatement = CurrentStatement?.Parent;

            CurrentExpression = null;
        }

        public void AttachExpression(QxExpression expression)
        {
            if (CurrentExpression is not null)
            {
                expression.Span = CurrentExpression.Span;
                CurrentExpression.Expression = expression;
            }
        }

        public void BeginExpression()
        {
            var newExpression = new ExpressionContext();

            CurrentStatement?.Expressions.Add(newExpression);

            CurrentExpression?.Children.Add(newExpression);
            newExpression.Parent = CurrentExpression;

            CurrentExpression = newExpression;
        }



        public void EndExpression()
        {
            CurrentExpression = CurrentExpression?.Parent;
        }

        public void ConsumeToken(Token token)
        {
            CurrentStatement?.Tokens.Add(token);
            CurrentExpression?.Tokens.Add(token);
        }
    }
}
