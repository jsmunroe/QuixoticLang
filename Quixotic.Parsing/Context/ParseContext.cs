using Quixotic.Common.Diagnostics;
using Quixotic.Common.Diagnostics.Issues;
using Quixotic.Common.Exceptions.Parsing;
using Quixotic.Common.Expressions;
using Quixotic.Common.Statements;
using Quixotic.Common.Tokens;

namespace Quixotic.Parsing.Context
{
    public class ParseContext : IParseContext
    {
        public List<StatementContext> _statements = [];

        public StatementContext? CurrentStatement { get; private set; }

        public ActivityContext? CurrentActivity { get; private set; }

        public void BeginStatement()
        {
            var newStatement = new StatementContext();

            if (CurrentStatement is null)
                _statements.Add(newStatement);

            CurrentStatement?.Children.Add(newStatement);
            newStatement.Parent = CurrentStatement;

            CurrentStatement = newStatement;
        }

        public void AssignStatementType(StatementType type)
        {
            CurrentStatement?.Type = type;
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
        }

        public void BeginActivity(ActivityType activityType)
        {
            var activity = new ActivityContext(activityType);

            CurrentStatement?.Activities.Add(activity);

            CurrentActivity?.Children.Add(activity);
            activity.Parent = CurrentActivity;

            CurrentActivity = activity;
        }
        public void AttachExpression(QxExpression expression)
        {
            if (CurrentActivity is not null)
            {
                expression.Span = CurrentActivity.Span;
                CurrentActivity.Expression = expression;
            }
        }

        public void EndActivity()
        {
            CurrentActivity = CurrentActivity?.Parent;
        }

        public void ConsumeToken(Token token)
        {
            CurrentStatement?.Tokens.Add(token);

            var currentStatement = CurrentStatement?.Parent;
            while (currentStatement is not null)
            {
                currentStatement.Tokens.Add(token);
                currentStatement = currentStatement.Parent;
            }

            CurrentActivity?.Tokens.Add(token);

            var currentActivity = CurrentActivity?.Parent;
            while (currentActivity is not null)
            {
                currentActivity.Tokens.Add(token);
                currentActivity = currentActivity.Parent;
            }
        }

        public Diagnostic GetDiagnostic(Issue issue)
        {
            var currentStatement = CurrentStatement;
            while (currentStatement?.Type == StatementType.Unknown)
                currentStatement = currentStatement.Parent;

            currentStatement ??= CurrentStatement;

            var currentActivity = CurrentActivity;
            while (currentActivity?.Type == ActivityType.None)
                currentActivity = currentActivity.Parent;

            currentActivity ??= CurrentActivity;

            return new Diagnostic(ContextType.Parsing, issue, currentStatement, CurrentActivity);
        }

        public Exception? GetExceptionForUnexpectedToken(Token token)
        {
            if (CurrentStatement is null)
                return new UnexpectedTokenException(token, GetDiagnostic(Issue.UnexpectedToken(token)));

            var isEol = token.Type == TokenType.NewLine || token.Type == TokenType.Eof;

            var tokens = CurrentStatement.Tokens;

            string description = CurrentStatement.Type switch
            {
                StatementType.Print => isEol ? "Unexpected end of statement after print. Expected an expression to print." : $"An unexpected character '{token.Value}' was encountered after the print statement. This cannot be printed.",
                StatementType.Identifier => isEol ? $"Unexpected end of statement after identifier '{GetLastIdentifierName(tokens)}'. Expected ':=' for an assignment." : $"An unexpected character '{token.Value}' was encountered after identifier '{GetLastIdentifierName(tokens)}'.",
                StatementType.Assignment => isEol ? $"Unexpected end of statement after assignment of '{GetLastIdentifierName(tokens)}'. Expected expression to assign to {GetLastIdentifierName(tokens)}." : $"An unexpected character '{token.Value}' is being assign to identifier '{GetLastIdentifierName(tokens)}'.",
                StatementType.If => throw new NotImplementedException(),
                StatementType.Do => throw new NotImplementedException(),
                StatementType.For => throw new NotImplementedException(),
                StatementType.Break => throw new NotImplementedException(),
                StatementType.Continue => throw new NotImplementedException(),
                StatementType.VariableDeclaration => throw new NotImplementedException(),
                StatementType.FunctionDeclaration => throw new NotImplementedException(),
                StatementType.FunctionCall => throw new NotImplementedException(),
                StatementType.Return => throw new NotImplementedException(),
                _ => throw new NotImplementedException(),
            };

            //var lineNumber = token.Span.Start.Line;
            //var sourceLine = _document.GetLine(lineNumber);
            //if (sourceLine is not null)
            //{
            //    var lineNumberText = $" {lineNumber}: ";
            //    var nextLineNumberText = $" {lineNumber + 1}: ";

            //    if (nextLineNumberText.Length > lineNumberText.Length)
            //        lineNumberText = lineNumberText + " ";

            //    var sourceLineText = sourceLine.Text.TrimEnd();
            //    var arrow = new string(' ', token.Span.Start.Column - 2) + '^';

            //    description += $"\r\n{lineNumberText}" + sourceLineText + $"\r\n{nextLineNumberText}" + arrow;
            //}

            return new ParserException(description, GetDiagnostic(Issue.UnexpectedToken(token)));
        }

        private string GetLastIdentifierName(IEnumerable<Token> tokens)
        {
            return tokens.LastOrDefault(t => t.Type == TokenType.Identifier)?.Value ?? "<unknown>";
        }
    }
}
