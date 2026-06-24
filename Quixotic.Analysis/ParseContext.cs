using Quixotic.Common.Expressions;
using Quixotic.Common.Statements;
using Quixotic.Common.Tokens;

namespace Quixotic.Analysis
{
    public class ParseContext
    {
        public List<DiagnosticStatement> _statements = [];

        public DiagnosticStatement? CurrentStatement { get; private set; }

        public DiagnosticExpression? CurrentExpression { get; private set; }

        public void BeginStatement()
        {
            var newStatement = new DiagnosticStatement();

            if (CurrentStatement is null)
                _statements.Add(newStatement);

            CurrentStatement?.Children.Add(newStatement);
            newStatement.Parent = CurrentStatement;

            CurrentStatement = newStatement;
        }

        public void EndStatement(QxStatement statement)
        {
            statement.Span = CurrentStatement!.Span;
            CurrentStatement = CurrentStatement?.Parent;

            CurrentExpression = null;
        }

        public void BeginExpression()
        {
            var newExpression = new DiagnosticExpression();

            CurrentStatement?.Expressions.Add(newExpression);

            CurrentExpression?.Children.Add(newExpression);
            newExpression.Parent = CurrentExpression;

            CurrentExpression = newExpression;
        }

        public TExpression CatchExpression<TExpression>(Func<TExpression> expressionFactory)
            where TExpression : QxExpression
        {
            BeginExpression();

            var expression = expressionFactory();

            EndExpression(expression);

            return expression;
        }

        public void EndExpression(QxExpression expression)
        {
            expression.Span = CurrentExpression!.Span;
            CurrentExpression = CurrentExpression?.Parent;
        }

        public void ConsumeToken(Token token)
        {
            CurrentStatement?.Tokens.Add(token);
            CurrentExpression?.Tokens.Add(token);
        }
    }

    public class DiagnosticStatement
    {
        public StatementType Type { get; set; }

        public List<Token> Tokens { get; } = [];

        public DiagnosticStatement? Parent { get; set; }

        public List<DiagnosticStatement> Children { get; } = [];

        public List<DiagnosticExpression> Expressions { get; } = [];

        public Span Span => Tokens.GetTotalSpan();
    }

    public class DiagnosticExpression
    {
        public List<Token> Tokens { get; } = [];

        public DiagnosticExpression? Parent { get; set; }

        public List<DiagnosticExpression> Children { get; } = [];

        public Span Span => Tokens.GetTotalSpan();
    }

    public enum StatementType
    {
        Unknown = 0,
        Print,
        Assignment,
        IfThen,
        DoWhile,
        For,
        Break,
        Continue,
        VariableDeclaration,
        FunctionDeclaration,
        FunctionCall,
        Return,

    }
}
