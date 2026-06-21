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
            : this(lexer.Tokenize())
        { }

        public IEnumerable<QxStatement> Parse()
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

        private QxStatement ParseStatement()
        {
            if (Match(TokenType.Print))
                return ParsePrint();

            if (Match(TokenType.Identifier, out var identifier))
                return ParseIdentifier(identifier);

            if (Match(TokenType.If))
                return ParseIf();

            if (Match(TokenType.Let))
                return ParseVariableDeclaration();

            if (Match(TokenType.Function))
                return ParseFunctionDeclaration();

            if (Match(TokenType.Return))
                return ParseReturn();

            throw new UnexpectedTokenException(_tokens.Peek());
        }

        private QxVariableDeclarationStatement ParseVariableDeclaration()
        {
            var identifierToken = Expect(TokenType.Identifier);

            var name = identifierToken.Value;

            QxExpression? expression = null;

            if (Match(TokenType.Assignment))
                expression = ParseExpression();

            return new(name, expression);
        }

        private QxFunctionDeclarationStatement ParseFunctionDeclaration()
        {
            var identifierToken = Expect(TokenType.Identifier);

            var name = identifierToken.Value;

            // Parse parameters
            var parameters = ParseParameters();

            var body = ParseBlock(() => false);

            Expect(TokenType.End);
            Expect(TokenType.Function);

            return new(name) { Parameters = [.. parameters], Body = body };
        }

        private QxReturnStatement ParseReturn()
        {
            if (Match(TokenType.NewLine))
                return new QxReturnStatement(null);

            var expression = ParseExpression();
            return new QxReturnStatement(expression);
        }

        private List<QxParameter> ParseParameters()
        {
            List<QxParameter> parameters = [];

            while (Match(TokenType.Identifier, out var token))
            {
                parameters.Add(new QxParameter(token.Value));

                if (Match(TokenType.NewLine))
                    break;

                Expect(TokenType.Comma);
            }

            return parameters;
        }

        private QxFunctionCallStatement ParseFunctionCall(Token token)
        {
            var name = token.Value;

            // Parse arguments
            var arguments = ParseArguments();

            return new(name) { Arguments = [.. arguments] };
        }
        private QxFunctionCallExpression ParseFunctionCallExpression(string name)
        {
            // Parse arguments
            var arguments = ParseArguments();

            return new(name) { Arguments = [.. arguments] };
        }

        private List<QxExpression> ParseArguments()
        {
            List<QxExpression> parameters = [];
            while (!Match(TokenType.CloseParen))
            {
                var expression = ParseExpression();
                parameters.Add(expression);

                if (Match(TokenType.CloseParen))
                    break;

                Expect(TokenType.Comma);
            }

            return parameters;
        }

        private QxPrintStatement ParsePrint()
        {
            var expression = ParseExpression();

            return new QxPrintStatement(expression);
        }

        private QxStatement ParseIdentifier(Token token)
        {
            var identifier = new QxIdentifierExpression(token.Value);

            var next = Pop();

            if (next.Type == TokenType.Assignment)
            {
                var expression = ParseExpression();

                return new QxAssignmentStatement(identifier, expression);
            }

            if (next.Type == TokenType.OpenParen)
            {
                return ParseFunctionCall(token);
            }

            throw new UnexpectedTokenException(next);
        }

        private QxIfStatement ParseIf()
        {
            var condition = ParseExpression();

            Allow(TokenType.Then); // Allowed as syntactic sugar, but not necessary

            if (!Match(TokenType.NewLine))
            {
                var statement = ParseStatement();

                return new QxIfStatement(condition) { ThenBlock = [statement] };
            }

            var thenBlock = ParseBlock(() => Peek().Type == TokenType.Else);

            List<QxElseIfClause> elseIfClauses = [];

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
                    elseBlock = ParseBlock(() => Peek().Type == TokenType.Else);
                }
            }

            Expect(TokenType.End);

            Expect(TokenType.If);

            return new QxIfStatement(condition)
            {
                ThenBlock = thenBlock,
                ElseIfClauses = elseIfClauses,
                ElseBlock = elseBlock,
            };
        }

        private QxElseIfClause ParseElseIfClause()
        {
            var condition = ParseExpression();

            var block = ParseBlock(() => Peek().Type == TokenType.Else);

            return new QxElseIfClause(condition)
            {
                Block = block,
            };
        }

        private QxExpression ParseExpression(int parentPrecedence = 0)
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

                left = new QxBinaryExpression(operationMetadata.Operator, left, right);
            }

            return left;
        }

        private QxExpression ParsePrimary()
        {
            var token = Pop();

            if (token.Type == TokenType.OpenParen)
            {
                var expression = ParseExpression();

                var nextToken = Pop();
                if (nextToken.Type != TokenType.CloseParen)
                    throw new ExpectedTokenException(new TokenHead(TokenType.CloseParen, ")"), nextToken);

                return expression;
            }

            if (token.Type == TokenType.StringLiteral)
                return new QxStringLiteralExpression(token.Value);

            if (token.Type == TokenType.NumberLiteral)
                return ParseNumber(token);

            if (token.Type == TokenType.Identifier)
                return ParseIdentifierExpression(token);

            throw new UnexpectedTokenException(token);
        }

        private QxExpression ParseIdentifierExpression(Token token)
        {
            var name = token.Value;

            if (Match(TokenType.OpenParen))
                return ParseFunctionCallExpression(name);

            return new QxIdentifierExpression(name);
        }

        private QxExpression ParseUnary()
        {
            if (Match(TokenType.Plus))
                return new QxUnaryExpression(Operator.Add, ParseUnary());

            if (Match(TokenType.Subtract))
                return new QxUnaryExpression(Operator.Subtract, ParseUnary());

            if (Match(TokenType.Not))
                return new QxUnaryExpression(Operator.Not, ParseUnary());

            return ParsePrimary();
        }

        private Block ParseBlock(Func<bool> terminationCondition)
        {
            var block = new Block();

            bool isTerminated() =>
                IsAtEnd ||
                Peek().Type == TokenType.End ||
                terminationCondition();

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

        private static QxNumberLiteralExpression ParseNumber(Token token)
        {
            if (double.TryParse(token.Value, out var number))
                return new QxNumberLiteralExpression(number);

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

        private bool TokenIs(TokenType type)
        {
            var token = Peek();
            return token.Type == type;
        }
    }
}
