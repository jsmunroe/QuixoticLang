using Quixotic.Common.Contracts;
using Quixotic.Common.Diagnostics;
using Quixotic.Common.Diagnostics.Issues;
using Quixotic.Common.Extensions;
using Quixotic.Common.Operations;
using Quixotic.Common.Tokens;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Quixotic.Analysis.Errors
{
    public class ErrorMessageFormatter
    {
        public bool PrintTokensAsValues { get; set; } = false;

        public string Describe(Exception exception)
        {
            if (exception is not IHasDiagnostic diagnosticSource)
                return exception.Message;

            var diagnostic = diagnosticSource.Diagnostic;

            var description = new StringBuilder();

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
                    description.Append(exception.Message);
                    break;
            }

            return description.ToString().Trim();
        }

        private void DescribeLexical(StringBuilder description, Exception exception, Diagnostic diagnostic)
        {
            description.Append(exception.Message);
        }

        private void DescribeParsing(StringBuilder description, Exception exception, Diagnostic diagnostic)
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

                default:
                    description.Append(exception.Message);
                    break;
            }
        }

        private void DescribeInterpretation(StringBuilder description, Exception exception, Diagnostic diagnostic)
        {
            description.Append(exception.Message);
        }

        private void DescribeParsingUnexpectedToken(StringBuilder description, Exception exception, UnexpectedToken issue, Diagnostic diagnostic)
        {
            switch (diagnostic.StatementType)
            {
                case StatementType.Print:
                    DescribeParsingUnexpectedTokenInPrint(description, exception, issue, diagnostic);
                    break;

                case StatementType.Assignment:
                    DescribeParsingUnexpectedTokenInAssignment(description, exception, issue, diagnostic);
                    break;

                case StatementType.If:
                    DescribeParsingUnexpectedTokenInIf(description, exception, issue, diagnostic);
                    break;

                case StatementType.Do:
                    DescribeParsingUnexpectedTokenInDo(description, exception, issue, diagnostic);
                    break;

                case StatementType.For:
                    DescribeParsingUnexpectedTokenInFor(description, exception, issue, diagnostic);
                    break;

                case StatementType.Break:
                    throw new NotImplementedException();
                case StatementType.Continue:
                    throw new NotImplementedException();

                case StatementType.VariableDeclaration:
                    DescribeParsingUnexpectedTokenInVariableDeclaration(description, exception, issue, diagnostic);
                    break;

                case StatementType.FunctionDeclaration:
                    DescribeParsingUnexpectedTokenInFunctionDeclaration(description, exception, issue, diagnostic);
                    break;

                case StatementType.FunctionCall:
                    DescribeParsingUnexpectedTokenInFunctionCall(description, exception, issue, diagnostic);
                    break;

                case StatementType.Return:
                    throw new NotImplementedException();
                case StatementType.Identifier:
                    DescribeParsingUnexpectedTokenInIdentifier(description, exception, issue, diagnostic);
                    break;
                case StatementType.Unknown:
                    DescribeParsingUnexpectedTokenInUnknown(description, exception, issue, diagnostic);
                    break;
                default:
                    description.Append(exception.Message);
                    break;
            }
        }

        private void DescribeParsingUnexpectedTokenInPrint(StringBuilder description, Exception exception, UnexpectedToken issue, Diagnostic diagnostic)
        {
            if (diagnostic.IsEndOfLine)
                description.Append($"An unexpected end of line was encountered while parsing the print statement. ");
            else if (diagnostic.IsRootActivity)
                description.Append($"An unexpected {DescribeToken(issue.Encountered)} was encountered while parsing the print statement. ");

            DescribeActivity(description, diagnostic.ActivityType, diagnostic, issue);

            if (issue.Expected is not null)
                description.Append($"The parser expected {DescribeToken(issue.Expected.Value)}. ");

            return;
        }

        private void DescribeParsingUnexpectedTokenInAssignment(StringBuilder description, Exception exception, UnexpectedToken issue, Diagnostic diagnostic)
        {
            var identifiers = diagnostic.Statement?.Tokens.FindValues(TokenType.Identifier) ?? [];
            var identifier = identifiers.LastOrDefault();

            if (identifier is not null)
                identifier = $" '{identifier}'";

            if (diagnostic.IsEndOfLine)
                description.Append($"An unexpected end of line was encountered while parsing the assignment of identifier{identifier}. ");
            else
                description.Append($"An unexpected {DescribeToken(issue.Encountered)} was encountered while parsing the assignment of identifier{identifier}. ");

            DescribeActivity(description, diagnostic.ActivityType, diagnostic, issue);

            if (issue.Expected is not null)
                description.Append($"The parser expected {DescribeToken(issue.Expected.Value).PrependIndefiniteArticle()}. ");
        }

        private void DescribeParsingUnexpectedTokenInIf(StringBuilder description, Exception exception, UnexpectedToken issue, Diagnostic diagnostic)
        {

            if (diagnostic.ActivityType == ActivityType.IfCondition)
            {
                if (diagnostic.IsEndOfLine)
                {
                    description.Append($"An unexpected end of line was encountered while parsing the condition of the if statement. ");
                    DescribeActivity(description, diagnostic.ActivityType, diagnostic, issue);

                    var expression = diagnostic.RootActivity?.Tokens.Any(t => !t.IsTerminator) == true ? diagnostic.RootActivity.Tokens.GetValue() : null;

                    if (expression is not null)
                        expression = $" '{expression}'";

                    description.Append($"The if condition expression{expression} may not be finished. ");
                }
                else
                {
                    description.Append($"An unexpected {DescribeToken(issue.Encountered)} was encountered while parsing the condition of the if statement. ");
                }
            }
            else
            {
                if (diagnostic.IsEndOfLine)
                    description.Append($"An unexpected end of line was encountered while parsing the if statement. ");
                else
                    description.Append($"An unexpected {DescribeToken(issue.Encountered)} was encountered while parsing the if statement. ");
            }


            if (issue.Expected is not null)
                description.Append($"The parser expected {DescribeToken(issue.Expected.Value).PrependIndefiniteArticle()}. ");
        }

        private void DescribeParsingUnexpectedTokenInDo(StringBuilder description, Exception exception, UnexpectedToken issue, Diagnostic diagnostic)
        {
            if (diagnostic.ActivityType == ActivityType.DoPrecondition || diagnostic.ActivityType == ActivityType.DoPostcondition)
            {
                if (diagnostic.IsEndOfLine)
                    description.Append($"An unexpected end of line was encountered while parsing the condition of the do statement. ");
                else
                    description.Append($"An unexpected {DescribeToken(issue.Encountered)} was encountered while parsing the condition of the do statement. ");

                DescribeActivity(description, diagnostic.ActivityType, diagnostic, issue);
            }
            else
            {
                if (diagnostic.IsEndOfLine)
                    description.Append($"An unexpected end of line was encountered while parsing the do statement. ");
                else
                    description.Append($"An unexpected {DescribeToken(issue.Encountered)} was encountered while parsing the do statement. ");
            }

            if (issue.Expected is not null)
                description.Append($"The parser expected {DescribeToken(issue.Expected.Value).PrependIndefiniteArticle()}. ");
        }

        private void DescribeParsingUnexpectedTokenInFor(StringBuilder description, Exception exception, UnexpectedToken issue, Diagnostic diagnostic)
        {
            if (diagnostic.ActivityType == ActivityType.Iterator)
            {
                if (diagnostic.IsEndOfLine)
                    description.Append($"An unexpected end of line was encountered while parsing the iterator of the for statement. ");
                else
                    description.Append($"An unexpected {DescribeToken(issue.Encountered)} was encountered while parsing the iterator of the for statement. ");

                DescribeActivity(description, diagnostic.ActivityType, diagnostic, issue);
            }
            else if (diagnostic.ActivityType == ActivityType.FromValue)
            {
                if (diagnostic.IsEndOfLine)
                    description.Append($"An unexpected end of line was encountered while parsing the from value of the for statement. ");
                else
                    description.Append($"An unexpected {DescribeToken(issue.Encountered)} was encountered while parsing the from value of the for statement. ");

                DescribeActivity(description, diagnostic.ActivityType, diagnostic, issue);
            }
            else if (diagnostic.ActivityType == ActivityType.ToValue)
            {
                if (diagnostic.IsEndOfLine)
                    description.Append($"An unexpected end of line was encountered while parsing the to value of the for statement. ");
                else
                    description.Append($"An unexpected {DescribeToken(issue.Encountered)} was encountered while parsing the to value of the for statement. ");

                DescribeActivity(description, diagnostic.ActivityType, diagnostic, issue);
            }
            else if (diagnostic.ActivityType == ActivityType.StepValue)
            {
                if (diagnostic.IsEndOfLine)
                    description.Append($"An unexpected end of line was encountered while parsing the step value of the for statement. ");
                else
                    description.Append($"An unexpected {DescribeToken(issue.Encountered)} was encountered while parsing the step value of the for statement. ");

                DescribeActivity(description, diagnostic.ActivityType, diagnostic, issue);
            }
            else
            {
                if (diagnostic.IsEndOfLine)
                    description.Append($"An unexpected end of line was encountered while parsing the for statement. ");
                else
                    description.Append($"An unexpected {DescribeToken(issue.Encountered)} was encountered while parsing the for statement. ");
            }

            if (issue.Expected is not null)
                description.Append($"The parser expected {DescribeToken(issue.Expected.Value).PrependIndefiniteArticle()}. ");
        }

        private void DescribeParsingUnexpectedTokenInIdentifier(StringBuilder description, Exception exception, UnexpectedToken issue, Diagnostic diagnostic)
        {
            var identifiers = diagnostic.Statement?.Tokens.FindValues(TokenType.Identifier) ?? [];
            var identifier = identifiers.LastOrDefault();

            if (identifier is not null)
                identifier = $" '{identifier}'";

            if (diagnostic.IsEndOfLine)
                description.Append($"An unexpected end of line was encountered while parsing the identifier{identifier}. ");
            else
                description.Append($"An unexpected {DescribeToken(issue.Encountered)} was encountered while parsing the identifier{identifier}. ");

            if (issue.Expected is not null)
                description.Append($"The parser expected {DescribeToken(issue.Expected.Value).PrependIndefiniteArticle()}. ");
        }

        private void DescribeParsingUnexpectedTokenInVariableDeclaration(StringBuilder description, Exception exception, UnexpectedToken issue, Diagnostic diagnostic)
        {
            if (diagnostic.IsEndOfLine)
                description.Append($"An unexpected end of line was encountered while parsing the variable declaration (let) statement. ");
            else
                description.Append($"An unexpected {DescribeToken(issue.Encountered)} was encountered while parsing the variable declaration (let) statement. ");

            DescribeActivity(description, diagnostic.ActivityType, diagnostic, issue);

            if (issue.Expected is not null)
                description.Append($"The parser expected {DescribeToken(issue.Expected.Value).PrependIndefiniteArticle()}. ");
        }

        private void DescribeParsingUnexpectedTokenInFunctionDeclaration(StringBuilder description, Exception exception, UnexpectedToken issue, Diagnostic diagnostic)
        {
            if (diagnostic.ActivityType == ActivityType.Parameter)
            {
                if (diagnostic.IsEndOfLine || issue.Encountered.IsTerminator)
                    description.Append($"An unexpected end of line was encountered while parsing the parameter list of the function declaration statement. ");
                else
                    description.Append($"An unexpected {DescribeToken(issue.Encountered)} was encountered while parsing the parameter list of the function declaration statement. ");
            }
            else
            {
                if (diagnostic.IsEndOfLine || issue.Encountered.IsTerminator)
                    description.Append($"An unexpected end of line was encountered while parsing the function declaration statement. ");
                else
                    description.Append($"An unexpected {DescribeToken(issue.Encountered)} was encountered while parsing the function declaration statement. ");
            }

            DescribeActivity(description, diagnostic.ActivityType, diagnostic, issue);

            if (issue.Expected is not null)
                description.Append($"The parser expected {DescribeToken(issue.Expected.Value).PrependIndefiniteArticle()}. ");
        }

        private void DescribeParsingUnexpectedTokenInFunctionCall(StringBuilder description, Exception exception, UnexpectedToken issue, Diagnostic diagnostic)
        {
            if (diagnostic.ActivityType == ActivityType.Argument)
            {
                if (diagnostic.IsEndOfLine)
                    description.Append($"An unexpected end of line was encountered while parsing the argument list of the function call statement. ");
                else
                    description.Append($"An unexpected {DescribeToken(issue.Encountered)} was encountered while parsing the argument list of the function call statement. ");
            }
            else
            {
                if (diagnostic.IsEndOfLine)
                    description.Append($"An unexpected end of line was encountered while parsing the function call statement. ");
                else
                    description.Append($"An unexpected {DescribeToken(issue.Encountered)} was encountered while parsing the function call statement. ");
            }

            DescribeActivity(description, diagnostic.ActivityType, diagnostic, issue);

            if (issue.Expected is not null)
                description.Append($"The parser expected {DescribeToken(issue.Expected.Value).PrependIndefiniteArticle()}. ");
        }

        private void DescribeParsingUnexpectedTokenInUnknown(StringBuilder description, Exception exception, UnexpectedToken issue, Diagnostic diagnostic)
        {
            if (diagnostic.IsEndOfLine)
                description.Append($"An unexpected end of line was encountered while parsing the unknown statement.");
            else
                description.Append($"An unexpected {DescribeToken(issue.Encountered)} was encountered while parsing the unknown statement.");

            if (issue.Expected is not null)
                description.Append($"The parser expected {DescribeToken(issue.Expected.Value).PrependIndefiniteArticle()}. ");

            return;
        }

        private void DescribeParsingIncompleteSource(StringBuilder description, Exception exception, IncompleteSource issue, Diagnostic diagnostic)
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

                case StatementType.Break:
                    throw new NotImplementedException();
                case StatementType.Continue:
                    throw new NotImplementedException();

                case StatementType.FunctionDeclaration:
                    DescribeParsingIncompleteSourceInFunctionDeclaration(description, exception, issue, diagnostic);
                    break;

                case StatementType.Return:
                    throw new NotImplementedException();
                default:
                    description.Append(exception.Message);
                    break;
            }
        }

        private void DescribeParsingIncompleteSourceInIf(StringBuilder description, Exception exception, IncompleteSource issue, Diagnostic diagnostic)
        {
            description.Append("Encountered end of file before if block has been properly terminated (end if).");
        }

        private void DescribeParsingIncompleteSourceInDo(StringBuilder description, Exception exception, IncompleteSource issue, Diagnostic diagnostic)
        {
            description.Append("Encountered end of file before do block has been properly terminated (end do).");
        }

        private void DescribeParsingIncompleteSourceInFor(StringBuilder description, Exception exception, IncompleteSource issue, Diagnostic diagnostic)
        {
            description.Append("Encountered end of file before for block has been properly terminated (next).");
        }

        private void DescribeParsingIncompleteSourceInFunctionDeclaration(StringBuilder description, Exception exception, IncompleteSource issue, Diagnostic diagnostic)
        {
            description.Append("Encountered end of file before function declaration has been properly terminated (end function).");
        }

        private void DescribeParsingDoLoopNoCondition(StringBuilder description, Exception exception, DoLoopNoCondition doLoopNoCondition, Diagnostic diagnostic)
        {
            description.Append("A do statement is lacking a condition. A do loop statement must have either a precondition (do while/until) or a postcondition (loop while/until) followed by a Boolean expression.");
        }

        private void DescribeParsingDoLoopDualCondition(StringBuilder description, Exception exception, DoLoopDualCondition doLoopDualCondition, Diagnostic diagnostic)
        {
            description.Append("A do statement cannot have both a precondition and postcondition expression.");
        }


        private void DescribeActivity(StringBuilder description, ActivityType activityType)
        {
            if (activityType == ActivityType.ParenSet)
                description.Append($"An open parenthesis has been left without a matching close parenthesis. ");

            if (activityType == ActivityType.UnaryPlus)
                description.Append($"An unary plus operator (+n) was encountered without an operand. ");

            if (activityType == ActivityType.UnaryNegation)
                description.Append($"An unary negation operator (-n) was encountered without an operand. ");

            if (activityType == ActivityType.UnaryNot)
                description.Append($"An unary not operator (not n) was encountered without an operand. ");
        }

        private void DescribeActivity(StringBuilder description, ActivityType activityType, Diagnostic diagnostic, UnexpectedToken issue)
        {
            DescribeActivity(description, activityType);

            if (activityType == ActivityType.RightOperand)
            {
                if (!diagnostic.IsEndOfLine)
                    description.Append($"{DescribeToken(issue.Encountered).Capitalize()} is an invalid operand in the expresssion '{diagnostic.RootActivity?.Tokens.GetValue()}'. ");
                else
                    description.Append($"The expression '{diagnostic.RootActivity?.Tokens.GetValue()}' may not be finished. ");
            }

        }

        private string DescribeToken(Token token)
        {
            if (IsKeyword(token.Type))
                return $"keyword '{token.Value}'";

            if (IsOperator(token.Type))
                return $"operator '{token.Value}'";

            return TokenToString(token);
        }

        private string DescribeToken(TokenType tokenType)
        {
            if (IsKeyword(tokenType))
                return $"keyword '{tokenType.ToString().ToLower()}'";

            if (TryGetOperatorValue(tokenType, out var value))
                return $"operator '{value}'";

            return TokenToString(tokenType);
        }

        private static bool IsKeyword(TokenType tokenType)
        {
            // TODO: Find a more robust way to determine if the token is a keyword. This is a placeholder implementation.

            TokenType[] keywordTypes = [
                TokenType.Print,
                TokenType.Let,
                TokenType.If,
                TokenType.Then,
                TokenType.Else,
                TokenType.For,
                TokenType.To,
                TokenType.Step,
                TokenType.Next,
                TokenType.Do,
                TokenType.While,
                TokenType.Until,
                TokenType.Loop,
                TokenType.Continue,
                TokenType.Break,
                TokenType.End,
                TokenType.BooleanLiteral,
                TokenType.And,
                TokenType.Or,
                TokenType.Not,
                TokenType.Function,
                TokenType.Return,
            ];

            if (keywordTypes.Contains(tokenType))
                return true;

            return false;
        }

        private static bool IsOperator(TokenType tokenType)
        {
            // TODO: Find a more robust way to determine if the token is an operator. This is a placeholder implementation.

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
