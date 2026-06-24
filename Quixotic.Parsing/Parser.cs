using Quixotic.Common.Exceptions.Parsing;
using Quixotic.Common.Expressions;
using Quixotic.Common.Operations;
using Quixotic.Common.Statements;
using Quixotic.Common.Tokens;
using QuixoticLang.Lexer;
using System.Diagnostics.CodeAnalysis;

namespace Quixotic.Parsing
{
    public class Parser(IEnumerable<Token> tokens)
    {
        private readonly Stepper<Token> _tokens = new(tokens);

        public Parser(string source)
            : this(new Lexer(source))
        { }

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

            if (Match(TokenType.For))
                return ParseFor();

            if (Match(TokenType.Do))
                return ParseDo();

            if (Match(TokenType.Let))
                return ParseVariableDeclaration();

            if (Match(TokenType.Function))
                return ParseFunctionDeclaration();

            if (Match(TokenType.Return))
                return ParseReturn();

            if (Match(TokenType.Break))
                return ParseBreak();

            if (Match(TokenType.Continue))
                return ParseContinue();

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

            var body = ParseBlock(TokenType.End);

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

        private QxBreakStatement ParseBreak()
        {
            return new();
        }

        private QxContinueStatement ParseContinue()
        {
            return new();
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
                if (Match(TokenType.Eof))
                    throw new IncompleteSourceException("End of file encountered before if block terminates.");

                var statement = ParseStatement();

                return new QxIfStatement(condition) { ThenBlock = [statement] };
            }

            var thenBlock = ParseBlock(TokenType.End, TokenType.Else);

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
                    elseBlock = ParseBlock(TokenType.End, TokenType.Else);
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

            var block = ParseBlock(TokenType.End, TokenType.Else);

            return new QxElseIfClause(condition)
            {
                Block = block,
            };
        }

        private QxForStatement ParseFor()
        {
            var identifierToken = Expect(TokenType.Identifier);
            var identifier = new QxIdentifierExpression(identifierToken.Value);

            Expect(TokenType.Assignment);

            var from = ParseExpression();

            Expect(TokenType.To);

            var to = ParseExpression();

            QxExpression step;
            if (Match(TokenType.Step))
                step = ParseExpression();
            else
                step = new QxNumberLiteralExpression(1);

            var block = ParseBlock(TokenType.Next);

            Expect(TokenType.Next);

            return new(identifier, from, to)
            {
                Step = step,
                Block = block,
            };
        }

        private QxDoStatement ParseDo()
        {
            var isEntryControlled = false;
            QxExpression? condition = null;
            if (Match(TokenType.While))
            {
                isEntryControlled = true;
                condition = ParseExpression();
            }
            else if (Match(TokenType.Until))
            {
                isEntryControlled = true;
                condition = ParseExpression();
                condition = new QxUnaryExpression(Operator.Not, condition);
            }

            var block = ParseBlock(TokenType.Loop);

            Expect(TokenType.Loop);

            if (Match(TokenType.While))
            {
                if (condition is not null)
                    throw new DoLoopDualConditionException();

                isEntryControlled = false;
                condition = ParseExpression();
            }
            else if (Match(TokenType.Until))
            {
                if (condition is not null)
                    throw new DoLoopDualConditionException();

                isEntryControlled = false;
                condition = ParseExpression();
                condition = new QxUnaryExpression(Operator.Not, condition);
            }

            if (condition is null)
                throw new DoLoopNoConditionException();

            return new(condition, isEntryControlled)
            {
                Block = [.. block],
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

            if (token.Type == TokenType.BooleanLiteral)
                return new QxBooleanLiteralExpression(string.Equals(token.Value, "true", StringComparison.OrdinalIgnoreCase));

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

        private Block ParseBlock(params TokenType[] terminatingTypes)
        {
            var block = new Block();

            bool isTerminated()
            {
                if (IsAtEnd || Match(TokenType.Eof))
                    throw new IncompleteSourceException("Encountered end of file before block is terminated.");

                return terminatingTypes.Any(t => Peek().Type == t);
            }

            while (!isTerminated())
            {
                ConsumeNewLines();

                if (IsAtEnd || Match(TokenType.Eof))
                    throw new IncompleteSourceException("Encountered end of file before block is terminated.");

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

            if (Match(TokenType.Eof))
                throw new IncompleteSourceException($"Encountered end of file before expected {type}.");

            throw new ExpectedTokenException(new TokenHead(type, type.ToString()), Peek());
        }

        private bool Match(TokenType type)
        {
            return Match(type, out _);
        }

        private bool Match(TokenType type, [NotNullWhen(true)] out Token? token)
        {
            if (IsAtEnd)
            {
                token = null;
                return false;
            }

            token = Peek();
            if (!_tokens.IsAtEnd && token.Type == type)
            {
                Advance();
                return true;
            }

            token = null;
            return false;
        }

        public static bool IsSourceComplete(string source)
        {
            try
            {
                var parser = new Parser(source);
                parser.Parse().ToList();
            }
            catch (IncompleteSourceException)
            {
                return false;
            }

            return true;
        }

    }
}
