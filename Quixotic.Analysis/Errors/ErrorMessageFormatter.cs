using Quixotic.Analysis.Exceptions;
using Quixotic.Common.Contracts;
using Quixotic.Common.Diagnostics;
using Quixotic.Common.Diagnostics.Issues;
using Quixotic.Common.Extensions;
using Quixotic.Common.Operations;
using Quixotic.Common.Source;
using Quixotic.Common.Syntax;
using Quixotic.Common.Tokens;
using Quixotic.Common.Utilities;
using System.Diagnostics.CodeAnalysis;

namespace Quixotic.Analysis.Errors
{
    public class ErrorMessageFormatter
    {
        public bool PrintTokensAsValues { get; set; } = false;

        public ConsoleText Describe(Exception exception, ISource source)
        {
            var description = Describe(exception);

            var span = Span.Empty;

            if (exception is IHasDiagnostic hasDiagnostic)
                span = hasDiagnostic.Diagnostic.Span;

            if (exception is SemanticException semanticException)
                span = semanticException.Span;

            if (span.IsEmpty)
                return description;

            SourceDocument sourceDocument = new(source);

            var sourceLines = sourceDocument.GetLines(span);

            description.WriteLine();
            description.WriteLine(sourceLines);

            return description;
        }

        public ConsoleText Describe(Exception exception)
        {
            if (exception is IHasDiagnostic diagnosticSource)
                return Describe(exception, diagnosticSource.Diagnostic);

            if (exception is SemanticException semanticException)
                return Describe(semanticException);

            return new ConsoleText(exception.Message);
        }

        public ConsoleText Describe(SemanticException semanticException)
        {
            var description = new ConsoleText();

            var color = semanticException.Severity switch
            {
                Semantics.Severity.Warning => ConsoleColor.Yellow,
                Semantics.Severity.Error => ConsoleColor.Red,
                _ => ConsoleColor.Gray,
            };

            description.Write($"{semanticException.Severity}: {semanticException.Message}", color);

            return description;
        }

        public ConsoleText Describe(Exception exception, Diagnostic diagnostic)
        {
            var description = new ConsoleText();

            switch (diagnostic.ContextType)
            {
                case ContextType.Lexical:
                    DescribeLexical(description, exception, diagnostic);
                    break;

                case ContextType.Parsing:
                    DescribeParsing(description, exception, diagnostic);
                    break;

                case ContextType.Interpretation:
                    DescribeInterpretation(description, exception, diagnostic);
                    break;

                default:
                    description.Write(exception.Message);
                    break;
            }

            return description;
        }

        private void DescribeLexical(ConsoleText description, Exception exception, Diagnostic diagnostic)
        {
            description.Write(exception.Message);
        }

        private void DescribeParsing(ConsoleText description, Exception exception, Diagnostic diagnostic)
        {
            switch (diagnostic.Issue)
            {
                case UnexpectedToken unexpectedToken:
                    DescribeParsingUnexpectedToken(description, exception, unexpectedToken, diagnostic);
                    break;

                case IncompleteSource incompleteSource:
                    DescribeParsingIncompleteSource(description, exception, incompleteSource, diagnostic);
                    break;

                case DoLoopNoCondition doLoopNoCondition:
                    DescribeParsingDoLoopNoCondition(description, exception, doLoopNoCondition, diagnostic);
                    break;

                case DoLoopDualCondition doLoopDualCondition:
                    DescribeParsingDoLoopDualCondition(description, exception, doLoopDualCondition, diagnostic);
                    break;

                case StandaloneExpression standaloneExpression:
                    DescribeParsingStandaloneExpression(description, exception, standaloneExpression, diagnostic);
                    break;

                default:
                    description.Write(exception.Message);
                    break;
            }
        }

        private void DescribeInterpretation(ConsoleText description, Exception exception, Diagnostic diagnostic)
        {
            description.Write(exception.Message);
        }

        private void DescribeParsingUnexpectedToken(ConsoleText description, Exception exception, UnexpectedToken issue, Diagnostic diagnostic)
        {
            var activity = diagnostic.Activity;

            List<ActivityType> secondaryActivityTypes = [
                ActivityType.UnaryNegation,
                ActivityType.UnaryNot,
                ActivityType.UnaryPlus,
                ActivityType.FunctionBody,
                ActivityType.ParenSet,
                ActivityType.RightOperand,
            ];
            while (activity is not null && secondaryActivityTypes.Contains(activity.Type))
                activity = activity.Parent;

            var activityType = activity?.Type;

            if (activityType is not null && activityType != ActivityType.None)
            {
                if (diagnostic.IsEndOfLine)
                    description.Write($"An unexpected end of line was encountered while parsing {DescribeActivityType(activityType.Value, diagnostic)}. ");
                else
                    description.Write($"An unexpected {DescribeToken(issue.Encountered)} was encountered while parsing {DescribeActivityType(activityType.Value, diagnostic)}. ");
            }
            else
            {
                if (diagnostic.IsEndOfLine)
                    description.Write($"An unexpected end of line was encountered while parsing {DescribeStatementType(diagnostic.StatementType, diagnostic)}. ");
                else
                    description.Write($"An unexpected {DescribeToken(issue.Encountered)} was encountered while parsing {DescribeStatementType(diagnostic.StatementType, diagnostic)}. ");
            }

            DescribeActivity(description, diagnostic.ActivityType, diagnostic, issue);

            if (issue.Expected is not null)
                description.Write($"The parser expected {DescribeToken(issue.Expected.Value).PrependIndefiniteArticle()}. ");
        }

        private void DescribeParsingIncompleteSource(ConsoleText description, Exception exception, IncompleteSource issue, Diagnostic diagnostic)
        {
            switch (diagnostic.StatementType)
            {

                case StatementType.If:
                    DescribeParsingIncompleteSourceInIf(description, exception, issue, diagnostic);
                    break;

                case StatementType.Do:
                    DescribeParsingIncompleteSourceInDo(description, exception, issue, diagnostic);
                    break;

                case StatementType.For:
                    DescribeParsingIncompleteSourceInFor(description, exception, issue, diagnostic);
                    break;

                case StatementType.FunctionDeclaration:
                    DescribeParsingIncompleteSourceInFunctionDeclaration(description, exception, issue, diagnostic);
                    break;

                default:
                    description.Write(exception.Message);
                    break;
            }
        }

        private void DescribeParsingIncompleteSourceInIf(ConsoleText description, Exception exception, IncompleteSource issue, Diagnostic diagnostic)
        {
            description.Write("Encountered end of file before if block has been properly terminated (end if).");
        }

        private void DescribeParsingIncompleteSourceInDo(ConsoleText description, Exception exception, IncompleteSource issue, Diagnostic diagnostic)
        {
            description.Write("Encountered end of file before do block has been properly terminated (end do).");
        }

        private void DescribeParsingIncompleteSourceInFor(ConsoleText description, Exception exception, IncompleteSource issue, Diagnostic diagnostic)
        {
            description.Write("Encountered end of file before for block has been properly terminated (next).");
        }

        private void DescribeParsingIncompleteSourceInFunctionDeclaration(ConsoleText description, Exception exception, IncompleteSource issue, Diagnostic diagnostic)
        {
            description.Write("Encountered end of file before function declaration has been properly terminated (end function).");
        }

        private void DescribeParsingDoLoopNoCondition(ConsoleText description, Exception exception, DoLoopNoCondition doLoopNoCondition, Diagnostic diagnostic)
        {
            description.Write("A do statement is lacking a condition. A do loop statement must have either a precondition (do while/until) or a postcondition (loop while/until) followed by a Boolean expression.");
        }

        private void DescribeParsingDoLoopDualCondition(ConsoleText description, Exception exception, DoLoopDualCondition doLoopDualCondition, Diagnostic diagnostic)
        {
            description.Write("A do statement cannot have both a precondition and postcondition expression.");
        }

        private void DescribeParsingStandaloneExpression(ConsoleText description, Exception exception, StandaloneExpression standaloneExpression, Diagnostic diagnostic)
        {
            description.Write($"The expression '{diagnostic.Statement?.Tokens.GetValue()}' cannot stand alone as a statement.");
        }

        private string DescribeStatementType(StatementType statementType, Diagnostic diagnostic)
        {
            string identifier()
            {
                var identifier = diagnostic.LastIdentifier;

                if (identifier is not null)
                    identifier = $" '{identifier}'";

                return identifier ?? string.Empty;
            }

            return statementType switch
            {
                StatementType.Print => "the print statement",
                StatementType.Identifier => $"the identifier{identifier()}",
                StatementType.Assignment => $"the assignment of identifier{identifier()}",
                StatementType.If => "the if statement",
                StatementType.Do => "the do statement",
                StatementType.For => "the for statement",
                StatementType.Break => "the break statement",
                StatementType.Continue => "the continue statement",
                StatementType.VariableDeclaration => "the variable declaration statement",
                StatementType.FunctionDeclaration => "the function declaration statement",
                StatementType.FunctionCall => "the function call statement",
                StatementType.Return => "the return statement",
                _ => "the statement",
            };
        }

        private string DescribeActivityType(ActivityType activityType, Diagnostic diagnostic)
        {
            string identifier()
            {
                var identifier = diagnostic.LastIdentifier;

                if (identifier is not null)
                    identifier = $" '{identifier}'";

                return identifier ?? string.Empty;
            }

            return activityType switch
            {
                ActivityType.Print => "the print statement",
                ActivityType.FromValue => "the from value of the for statement",
                ActivityType.ReturnValue => "the return value expression",
                ActivityType.Argument => "the argument expression",
                ActivityType.ElseIfCondition => "the else if condition",
                ActivityType.ToValue => "the to value of the for statement",
                ActivityType.StepValue => "the step value of the for statement",
                ActivityType.DoPostcondition => "the postcondition of the do statement",
                ActivityType.Identifier => $"the identifier{identifier()}",
                ActivityType.Parameter => "the parameter expression",
                ActivityType.ConsumeStatementTerminator => "the statement terminator",
                ActivityType.AssignedExpression => $"the assignment of identifier{identifier()}",
                ActivityType.IfCondition => "the if condition",
                ActivityType.IfThenBlock => "the then block",
                ActivityType.ElseBlock => "the else block",
                ActivityType.ElseIfBlock => "the else if block",
                ActivityType.DoPrecondition => "the precondition of the do statement",
                ActivityType.DoBlock => "the do block",
                ActivityType.ForBlock => "the for block",
                ActivityType.FunctionName => "the function name",
                ActivityType.FunctionBody => "the function body",
                ActivityType.ParenSet => "the set of parentheses",
                ActivityType.StringLiteral => "the string literal",
                ActivityType.BooleanLiteral => "the Boolean literal",
                ActivityType.NumberLiteral => "the number literal",
                ActivityType.UnaryPlus => "the unary plus expression",
                ActivityType.UnaryNegation => "the unary negation expression",
                ActivityType.UnaryNot => "the unary not expression",
                ActivityType.RightOperand => "the right operand",
                ActivityType.LeftOperand => "the left operand",
                ActivityType.FunctionReturnType => "the function return type",
                _ => "the expression",
            };
        }

        private void DescribeActivity(ConsoleText description, ActivityType activityType)
        {
            if (activityType == ActivityType.ParenSet)
                description.Write($"An open parenthesis has been left without a matching close parenthesis. ");

            if (activityType == ActivityType.UnaryPlus)
                description.Write($"A unary plus operator (+n) was encountered without an operand. ");

            if (activityType == ActivityType.UnaryNegation)
                description.Write($"A unary negation operator (-n) was encountered without an operand. ");

            if (activityType == ActivityType.UnaryNot)
                description.Write($"A unary not operator (not n) was encountered without an operand. ");
        }

        private void DescribeActivity(ConsoleText description, ActivityType activityType, Diagnostic diagnostic, UnexpectedToken issue)
        {
            DescribeActivity(description, activityType);

            if (activityType == ActivityType.RightOperand)
            {
                if (!diagnostic.IsEndOfLine)
                    description.Write($"{DescribeToken(issue.Encountered).Capitalize()} is an invalid operand in the expression '{GetExpressionValue(diagnostic)}'. ");
                else
                    description.Write($"The expression '{GetExpressionValue(diagnostic)}' may not be finished. ");
            }

            if (activityType == ActivityType.IfCondition && diagnostic.IsEndOfLine)
            {
                var expression = diagnostic.RootActivity?.Tokens.Any(t => !t.IsTerminator) == true ? GetExpressionValue(diagnostic) : null;

                if (expression is not null)
                    expression = $" '{expression}'";

                description.Write($"The if condition expression{expression} may not be finished. ");
            }

        }

        private string? GetExpressionValue(Diagnostic diagnostic)
        {
            return diagnostic.RootActivity?.Tokens.GetValue();
        }

        private string DescribeToken(Token token)
        {
            if (IsKeyword(token.Type))
                return $"keyword '{token.Value}'";

            if (IsOperator(token.Type))
                return $"operator '{token.Value}'";

            if (token.Type == TokenType.NewLine)
                return "end of line";

            return TokenToString(token);
        }

        private string DescribeToken(TokenType tokenType)
        {
            if (IsKeyword(tokenType))
                return $"keyword '{tokenType.ToString().ToLower()}'";

            if (TryGetOperatorValue(tokenType, out var value))
                return $"operator '{value}'";

            if (tokenType == TokenType.NewLine)
                return "end of the line";

            return TokenToString(tokenType);
        }

        private static bool IsKeyword(TokenType tokenType)
        {
            if (Keyword.Contains(tokenType))
                return true;

            return false;
        }

        private static bool IsOperator(TokenType tokenType)
        {
            var operationMetadata = OperationMetadata.Get(tokenType);

            return operationMetadata.Operator != Operator.None;
        }

        private static bool TryGetOperatorValue(TokenType tokenType, [NotNullWhen(returnValue: true)] out string? operatorValue)
        {
            var operationMetadata = OperationMetadata.Get(tokenType);
            if (operationMetadata.Operator != Operator.None)
            {
                operatorValue = operationMetadata.Value;
                return true;
            }

            operatorValue = null;
            return false;
        }

        private string TokenToString(Token? token)
        {
            if (token == null)
                return string.Empty; // TODO: Consider providing some default representation for null tokens.

            if (PrintTokensAsValues)
                return token.Value;

            return token.ToString();
        }

        private string TokenToString(TokenType? tokenType)
        {
            if (tokenType == null)
                return string.Empty; // TODO: Consider providing some default representation for null tokens.

            return tokenType.ToText().ToLower();
        }
    }
}
