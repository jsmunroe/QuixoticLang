using Quixotic.Lexography.Tokens;
using Quixotic.Parsing.Exceptions;
using Quixotic.Parsing.Expressions;
using Quixotic.Parsing.Operations;
using Quixotic.Parsing.Statements;
using QuixoticLang.Lexer;
using System.Diagnostics.CodeAnalysis;

namespace Quixotic.Parsing
{
    public class Parser(IEnumerable<Token> tokens)
    {
        private readonly Stepper<Token> _tokens = new(tokens);

        public Parser(Lexer lexer)
            : this(lexer.Run())
        { }

        public IEnumerable<Statement> Parse()
        {
            while (!_tokens.IsAtEnd && _tokens.Peek().Type != TokenType.Eof)
            {
                ConsumeNewLines();

                if (_tokens.IsAtEnd || _tokens.Peek().Type == TokenType.Eof)
                    yield break;

                var statement = ParseStatement();
                yield return statement;

                ConsumeStatementTerminator();
            }
        }

        private void ConsumeNewLines()
        {
            while (Match(TokenType.NewLine))
                ; // Consume new lines
        }

        private void ConsumeStatementTerminator()
        {
            if (_tokens.IsAtEnd)
                return;

            if (Match(TokenType.NewLine))
                return;

            if (Match(TokenType.Eof))
                return;

            throw new UnexpectedTokenException(_tokens.Peek());
        }

        public Statement ParseStatement()
        {
            if (Match(TokenType.Print))
                return ParsePrint();

            if (Match(TokenType.Identifier, out var identifier))
                return ParseIdentifier(identifier);

            throw new UnexpectedTokenException(_tokens.Peek());
        }

        public Statement ParsePrint()
        {
            var expression = ParseExpression();

            return new PrintStatement(expression);
        }

        public Statement ParseIdentifier(Token token)
        {
            var identifier = new IdentifierExpression(token.Value);

            var next = _tokens.Pop();

            if (next.Type == TokenType.Equals)
            {
                var expression = ParseExpression();

                return new AssignmentStatement(identifier, expression);
            }

            throw new UnexpectedTokenException(next);
        }

        public Expression ParseExpression(int parentPrecedence = 0)
        {
            var left = ParsePrimary();

            while (true)
            {
                var token = _tokens.Peek();

                var operationMetadata = OperationMetadata.Get(token.Type);

                var nextPrecedence = operationMetadata.Precedence;

                if (operationMetadata.Operator == Operator.None)
                    break;

                if (operationMetadata.Associativity == Associativity.Left)
                    nextPrecedence++;

                if (nextPrecedence <= parentPrecedence)
                    break;

                _tokens.Advance(); // consume operator

                var right = ParseExpression(operationMetadata.Precedence);

                left = new BinaryExpression(operationMetadata.Operator, left, right);
            }

            return left;
        }

        public Expression ParsePrimary()
        {
            var token = _tokens.Pop();

            if (token.Type == TokenType.LeftParen)
            {
                var expression = ParseExpression();

                var nextToken = _tokens.Pop();
                if (nextToken.Type != TokenType.RightParen)
                    throw new ExpectedTokenException(new TokenHead(TokenType.RightParen, ")"), nextToken);

                return expression;
            }

            if (token.Type == TokenType.StringLiteral)
                return new StringLiteralExpression(token.Value);

            if (token.Type == TokenType.NumberLiteral)
                return ParseNumber(token);

            if (token.Type == TokenType.Identifier)
                return new IdentifierExpression(token.Value);

            throw new UnexpectedTokenException(token);
        }

        private static NumberLiteralExpression ParseNumber(Token token)
        {
            if (double.TryParse(token.Value, out var number))
                return new NumberLiteralExpression(number);

            throw new TokenException("Invalid number literal", token);
        }

        private bool Match(TokenType type)
        {
            return Match(type, out _);
        }

        private bool Match(TokenType type, [NotNullWhen(true)] out Token? token)
        {
            token = _tokens.Peek();
            if (!_tokens.IsAtEnd && token.Type == type)
            {
                _tokens.Advance();
                return true;
            }

            token = null;
            return false;
        }
    }
}
