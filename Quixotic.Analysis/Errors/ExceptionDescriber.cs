using Quixotic.Common.Contracts;
using Quixotic.Common.Diagnostics;
using Quixotic.Common.Diagnostics.Issues;
using Quixotic.Common.Tokens;
using System.Text;

namespace Quixotic.Analysis.Errors
{
    public class ExceptionDescriber
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

            return description.ToString();
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
                    DescribeParsingUnexpectedToken(description, exception, diagnostic);
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

        public void DescribeParsingUnexpectedToken(StringBuilder description, Exception exception, Diagnostic diagnostic)
        {
            switch (diagnostic.StatementType)
            {
                case StatementType.Print:
                    DescribeParsingUnexpectedTokenInPrint(description, exception, diagnostic);
                    break;

                case StatementType.Assignment:
                    throw new NotImplementedException();
                case StatementType.If:
                    throw new NotImplementedException();
                case StatementType.Do:
                    throw new NotImplementedException();
                case StatementType.For:
                    throw new NotImplementedException();
                case StatementType.Break:
                    throw new NotImplementedException();
                case StatementType.Continue:
                    throw new NotImplementedException();
                case StatementType.VariableDeclaration:
                    throw new NotImplementedException();
                case StatementType.FunctionDeclaration:
                    throw new NotImplementedException();
                case StatementType.FunctionCall:
                    throw new NotImplementedException();
                case StatementType.Return:
                    throw new NotImplementedException();
                case StatementType.Identifier:
                    throw new NotImplementedException();
                default:
                    description.Append(exception.Message);
                    break;
            }
        }

        public void DescribeParsingUnexpectedTokenInPrint(StringBuilder description, Exception exception, Diagnostic diagnostic)
        {
            if (diagnostic.IsEndOfLine)
            {
                description.Append("The print statement is incomplete. It should be followed by an expression to print.");
                return;
            }

            description.Append($"An unexpected token '{TokenToString(diagnostic.LastConsumedToken)}' was encountered in the expression to print. This cannot be printed.");
        }

        private string TokenToString(Token? token)
        {
            if (token == null)
                return string.Empty; // TODO: Consider providing some default representation for null tokens.

            if (PrintTokensAsValues)
                return token.Value;

            return token.ToString();
        }
    }
}
