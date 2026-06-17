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
            while (!IsAtEnd && Peek().Type != TokenType.Eof)
            {
                ConsumeNewLines();

                if (IsAtEnd || Peek().Type == TokenType.Eof)
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
            if (IsAtEnd)
                return;

            if (Match(TokenType.NewLine))
                return;

            if (Match(TokenType.Eof))
                return;

            throw new UnexpectedTokenException(_tokens.Peek());
        }

        private Statement ParseStatement()
        {
            if (Match(TokenType.Print))
                return ParsePrint();

            if (Match(TokenType.Identifier, out var identifier))
                return ParseIdentifier(identifier);

            if (Match(TokenType.If))
                return ParseIf();

            throw new UnexpectedTokenException(_tokens.Peek());
        }

        private Statement ParsePrint()
        {
            var expression = ParseExpression();

            return new PrintStatement(expression);
        }

        private AssignmentStatement ParseIdentifier(Token token)
        {
            var identifier = new IdentifierExpression(token.Value);

            var next = Pop();

            if (next.Type == TokenType.Assignment)
            {
                var expression = ParseExpression();

                return new AssignmentStatement(identifier, expression);
            }

            throw new UnexpectedTokenException(next);
        }

        private IfStatement ParseIf()
        {
            var condition = ParseExpression();

            Allow(TokenType.Then); // Allowed as syntactic sugar, but not necessary

            if (!Match(TokenType.NewLine))
            {
                var statement = ParseStatement();

                return new IfStatement(condition) { ThenBlock = [statement] };
            }

            var thenBlock = ParseBlock(t => t.Type == TokenType.Else);

            List<ElseIfClause> elseIfClauses = [];

            Block elseBlock = [];

            while (Match(TokenType.Else))
            {
                if (Match(TokenType.If))
                {
                    var elseIfClasue = ParseElseIfClause();
                    elseIfClauses.Add(elseIfClasue);
                }
                else
                {
                    elseBlock = ParseBlock(t => t.Type == TokenType.Else);
                }
            }

            Expect(TokenType.End);

            Expect(TokenType.If);

            return new IfStatement(condition)
            {
                ThenBlock = thenBlock,
                ElseIfClauses = elseIfClauses,
                ElseBlock = elseBlock,
            };
        }

        private ElseIfClause ParseElseIfClause()
        {
            var condition = ParseExpression();

            var block = ParseBlock(t => t.Type == TokenType.Else);

            return new ElseIfClause(condition)
            {
                Block = block,
            };
        }

        private Expression ParseExpression(int parentPrecedence = 0)
        {
            var left = ParseUnary();

            while (true)
            {
                var token = Peek();

                var operationMetadata = OperationMetadata.Get(token.Type);

                var nextPrecedence = operationMetadata.Precedence;

                if (operationMetadata.Operator == Operator.None)
                    break;

                if (operationMetadata.Associativity == Associativity.Left)
                    nextPrecedence++;

                if (nextPrecedence <= parentPrecedence)
                    break;

                Advance(); // consume operator

                var right = ParseExpression(operationMetadata.Precedence);

                left = new BinaryExpression(operationMetadata.Operator, left, right);
            }

            return left;
        }

        private Expression ParsePrimary()
        {
            var token = Pop();

            if (token.Type == TokenType.LeftParen)
            {
                var expression = ParseExpression();

                var nextToken = Pop();
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

        private Expression ParseUnary()
        {
            if (Match(TokenType.Plus))
                return new UnaryExpression(Operator.Add, ParseUnary());

            if (Match(TokenType.Subtract))
                return new UnaryExpression(Operator.Subtract, ParseUnary());

            if (Match(TokenType.Not))
                return new UnaryExpression(Operator.Not, ParseUnary());

            return ParsePrimary();
        }

        private Block ParseBlock(Func<Token, bool> terminationCondition)
        {
            var block = new Block();

            bool isTerminated() =>
                IsAtEnd ||
                Peek().Type == TokenType.End ||
                terminationCondition(Peek());

            while (!isTerminated())
            {
                ConsumeNewLines();

                if (isTerminated())
                    break;

                var statement = ParseStatement();
                block.Add(statement);

                ConsumeStatementTerminator();
            }

            return block;
        }

        private static NumberLiteralExpression ParseNumber(Token token)
        {
            if (double.TryParse(token.Value, out var number))
                return new NumberLiteralExpression(number);

            throw new TokenException("Invalid number literal", token);
        }

        private bool IsAtEnd => _tokens.IsAtEnd;

        private Token Peek() => _tokens.Peek();

        private Token Pop() => _tokens.Pop();

        private bool Advance() => _tokens.Advance();

        private Token? Allow(TokenType type)
        {
            if (Match(type, out var token))
                return token;

            return null;
        }

        private Token Expect(TokenType type)
        {
            if (Match(type, out var token))
                return token;

            throw new ExpectedTokenException(new TokenHead(type, type.ToString()), Peek());
        }

        private bool Match(TokenType type)
        {
            return Match(type, out _);
        }

        private bool Match(TokenType type, [NotNullWhen(true)] out Token? token)
        {
            token = Peek();
            if (!_tokens.IsAtEnd && token.Type == type)
            {
                Advance();
                return true;
            }

            token = null;
            return false;
        }
    }
}
