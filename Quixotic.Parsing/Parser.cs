using Quixotic.Common.Diagnostics;
using Quixotic.Common.Diagnostics.Issues;
using Quixotic.Common.Exceptions.Parsing;
using Quixotic.Common.Expressions;
using Quixotic.Common.Operations;
using Quixotic.Common.Statements;
using Quixotic.Common.Symbols;
using Quixotic.Common.Tokens;
using Quixotic.Parsing.Context;
using QuixoticLang.Lexer;
using System.Diagnostics.CodeAnalysis;

namespace Quixotic.Parsing
{
    public class Parser(IEnumerable<Token> tokens)
    {
        private readonly Stepper<Token> _tokens = new(tokens);

        private readonly ParseContext _parseContext = new();

        public Parser(string source)
            : this(new Lexer(source))
        { }

        public Parser(Lexer lexer)
            : this(lexer.Tokenize())
        { }

#if DEBUG
        private Token? Current { get; set; }

        private int _previousCount = 10;
        private readonly Queue<Token> _previous = [];

        private string Previous => string.Join("; ", _previous.Reverse().Select(t => $"{{{t}}}"));
#endif
        public IEnumerable<QxStatement> Parse()
        {
            while (!IsAtEnd && Peek().Type != TokenType.Eof)
            {
                _parseContext.BeginStatement();

                ConsumeNewLines();

                if (IsAtEnd || Peek().Type == TokenType.Eof)
                    yield break;

                var statement = ParseStatement();

                _parseContext.AttachStatement(statement);
                yield return statement;

                ConsumeStatementTerminator();

                _parseContext.EndStatement();
            }
        }

        private void ConsumeNewLines()
        {
            while (Match(TokenType.NewLine))
                ; // Consume new lines
        }

        private void ConsumeStatementTerminator()
        {
            CaptureActivity(() =>
            {
                if (IsAtEnd)
                    return;

                if (Match(TokenType.NewLine))
                    return;

                if (Match(TokenType.Eof))
                    return;

                throw new UnexpectedTokenException(_tokens.Peek(), GetDiagnostic(Issue.UnexpectedToken(Peek(), TokenType.NewLine)));
            }, ActivityType.ConsumeStatementTerminator);
        }

        private QxStatement ParseStatement()
        {
            if (Match(TokenType.Print))
                return ParsePrint();

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

            if (Match(TokenType.Type))
                return ParseTypeDeclaration();

            return ParseStandalonExpression();

            throw new UnexpectedTokenException(_tokens.Peek(), GetDiagnostic(Issue.UnexpectedToken(Peek())));
        }

        private QxStatement ParseDeclarationStatement()
        {
            if (Match(TokenType.Let))
                return ParseVariableDeclaration();

            if (Match(TokenType.Function))
                return ParseFunctionDeclaration();

            if (Match(TokenType.Construct))
                return ParseConstructor();

            throw new UnexpectedTokenException(_tokens.Peek(), GetDiagnostic(Issue.UnexpectedToken(Peek())));
        }


        private QxStatement ParseStandalonExpression()
        {
            _parseContext.AssignStatementType(StatementType.StandaloneExpression);

            var expression = CaptureExpression(() => ParseExpression(), ActivityType.StandaloneExpression);

            return ParseStandalonExpression(expression);
        }

        private QxStatement ParseStandalonExpression(QxExpression expression)
        {
            _parseContext.AssignStatementType(StatementType.StandaloneExpression);

            if (expression is QxFunctionCallExpression functionCallExpression)
            {
                _parseContext.AssignStatementType(StatementType.FunctionCall);

                return new QxFunctionCallStatement(functionCallExpression);
            }

            if (expression is QxBinaryExpression binaryExpression &&
                binaryExpression.Operator == Operator.Assignment &&
                binaryExpression.Left is QxAssignableExpression assignableExpression)
            {
                _parseContext.AssignStatementType(StatementType.Assignment);

                return new QxAssignmentStatement(assignableExpression, binaryExpression.Right);
            }

            if (expression is QxMethodCallExpression methodCallExpression)
            {
                _parseContext.AssignStatementType(StatementType.MethodCall);

                return new QxMethodCallStatement(methodCallExpression);
            }

            throw new StandaloneExpressionException(GetDiagnostic(Issue.StandaloneExpression()));
        }

        private QxVariableDeclarationStatement ParseVariableDeclaration()
        {
            _parseContext.AssignStatementType(StatementType.VariableDeclaration);

            var name = CaptureActivity(() =>
            {
                var identifierToken = Expect(TokenType.Identifier);
                var name = identifierToken.Value;

                return name;

            }, ActivityType.Identifier);

            var typeName = CaptureActivity(() =>
            {
                if (Match(TokenType.Colon))
                    return ParseTypeName();

                return null;

            }, ActivityType.IdentifierType);

            QxExpression? expression = null;

            if (Match(TokenType.Assignment))
                expression = CaptureExpression(() => ParseExpression(), ActivityType.AssignedExpression);

            return new(name, typeName, expression);
        }

        private QxFunctionDeclarationStatement ParseFunctionDeclaration()
        {
            _parseContext.AssignStatementType(StatementType.FunctionDeclaration);

            var name = CaptureActivity(() =>
            {
                var identifierToken = Expect(TokenType.Identifier);
                var name = identifierToken.Value;

                return name;

            }, ActivityType.FunctionName);

            // Parse parameters
            List<QxParameter> parameters = [];
            if (Match(TokenType.OpenParen))
                parameters = ParseParameters();

            var returnType = CaptureActivity(() =>
            {
                if (Match(TokenType.Colon))
                    return ParseTypeName();

                return "void";

            }, ActivityType.FunctionReturnType);

            var body = CaptureActivity(() => ParseBlock(BlockType.Function), ActivityType.FunctionBody);

            Expect(TokenType.EndFunction);

            return new(name, returnType) { Parameters = [.. parameters], Body = body };
        }

        private QxReturnStatement ParseReturn()
        {
            _parseContext.AssignStatementType(StatementType.Return);

            if (Match(TokenType.NewLine))
                return new QxReturnStatement(null);

            var expression = CaptureExpression(() => ParseExpression(), ActivityType.ReturnValue);
            return new QxReturnStatement(expression);
        }

        private QxBreakStatement ParseBreak()
        {
            _parseContext.AssignStatementType(StatementType.Break);

            return new();
        }

        private QxContinueStatement ParseContinue()
        {
            _parseContext.AssignStatementType(StatementType.Continue);
            return new();
        }

        private QxTypeDeclarationStatement ParseTypeDeclaration()
        {
            var name = ParseIdentifierName();

            string baseName = "any";

            if (Match(TokenType.Is))
                baseName = ParseIdentifierName();

            var body = CaptureActivity(() => ParseDeclarationBlock(), ActivityType.TypeBody);

            Expect(TokenType.EndType);

            return new(name) { Body = body, BaseName = baseName };
        }

        private QxConstructorDeclarationStatement ParseConstructor()
        {
            _parseContext.AssignStatementType(StatementType.ConstructorDeclaration);

            // Parse parameters
            List<QxParameter> parameters = [];
            if (Match(TokenType.OpenParen))
                parameters = ParseParameters();

            QxBaseConstructorCallExpression? baseCall = null;

            if (Match(TokenType.Colon))
                baseCall = ParseBaseConstructorCall();

            var body = CaptureActivity(() => ParseBlock(BlockType.Constructor), ActivityType.FunctionBody);

            Expect(TokenType.EndConstruct);

            return new(baseCall) { Parameters = [.. parameters], Body = body };
        }

        private QxConstructorCallExpression ParseTypeInstantiation()
        {
            var typeName = ParseIdentifierName();

            // Parse parameters
            List<QxExpression> arguments = [];
            if (Match(TokenType.OpenParen))
                arguments = ParseArguments();

            return new(typeName)
            {
                Arguments = arguments
            };
        }

        private QxBaseConstructorCallExpression ParseBaseConstructorCall()
        {
            Expect(TokenType.Identifier); // base

            List<QxExpression>? arguments = [];
            if (Match(TokenType.OpenParen))
                arguments = ParseArguments();

            return new()
            {
                Arguments = [.. arguments],
            };
        }

        private List<QxParameter> ParseParameters()
        {
            List<QxParameter> parameters = [];

            var keepGoing = !Match(TokenType.CloseParen);

            while (keepGoing)
            {
                keepGoing = CaptureActivity(() =>
                {
                    if (Match(TokenType.Identifier, out var token))
                    {
                        Expect(TokenType.Colon);
                        var parameterTypeName = ParseTypeName();

                        parameters.Add(new QxParameter(token.Value, parameterTypeName));
                    }
                    else
                    {
                        throw new UnexpectedTokenException(Peek(), GetDiagnostic(Issue.UnexpectedToken(Peek(), TokenType.Identifier)));
                    }

                    if (Match(TokenType.CloseParen))
                        return false;

                    Expect(TokenType.Comma);

                    return true;

                }, ActivityType.Parameter);
            }

            return parameters;
        }

        private QxFunctionCallExpression ParseFunctionCallExpression(string name)
        {
            // Parse arguments
            var arguments = ParseArguments();

            return new(name) { Arguments = [.. arguments] };
        }

        private QxMethodCallExpression ParseMethodCallExpression(QxExpression target)
        {
            var token = Expect(TokenType.Identifier);

            var name = token.Value;

            if (Match(TokenType.Assignment))
            {
                var assigned = ParseExpression();

                return new(target, name, FunctionCallType.Setter) { Arguments = [assigned] };
            }
            else if (Match(TokenType.OpenParen))
            {
                // Parse arguments
                var arguments = ParseArguments();
                return new(target, name, FunctionCallType.Call) { Arguments = [.. arguments] };
            }

            return new(target, name, FunctionCallType.Getter) { Arguments = [] };
        }

        private List<QxExpression> ParseArguments()
        {
            var keepGoing = true;
            var afterComma = false;

            List<QxExpression> parameters = [];
            while (keepGoing)
            {
                keepGoing = CaptureActivity(() =>
                {
                    if (Match(TokenType.CloseParen, out var token))
                    {
                        if (!afterComma)
                            return false;

                        throw new UnexpectedTokenException(token, GetDiagnostic(Issue.UnexpectedToken(token, TokenType.Identifier)));
                    }

                    var expression = CaptureExpression(() => ParseExpression(), ActivityType.Argument);
                    parameters.Add(expression);

                    if (Match(TokenType.CloseParen))
                        return false;

                    Expect(TokenType.Comma);
                    afterComma = true;

                    return true;

                }, ActivityType.Argument);
            }

            return parameters;
        }

        private QxArrayExpression ParseArray()
        {
            if (IsToken(TokenType.CloseBracket))
                return new QxArrayExpression(); // Empty array

            List<QxExpression> elements = [];

            while (!IsToken(TokenType.CloseBracket))
            {
                var element = CaptureExpression(() => ParseExpression(), ActivityType.ArrayElement);

                elements.Add(element);

                Allow(TokenType.Comma);
            }

            return new QxArrayExpression
            {
                Elements = [.. elements],
            };
        }

        private QxSetExpression ParseSet()
        {
            if (IsToken(TokenType.CloseBrace))
                return new QxSetExpression(); // Empty array

            List<QxExpression> elements = [];

            while (!IsToken(TokenType.CloseBrace))
            {
                var element = CaptureExpression(() => ParseExpression(), ActivityType.ArrayElement);

                elements.Add(element);

                Allow(TokenType.Comma);
            }

            return new QxSetExpression
            {
                Elements = [.. elements],
            };
        }

        private QxPrintStatement ParsePrint()
        {
            _parseContext.AssignStatementType(StatementType.Print);

            var expression = CaptureExpression(() => ParseExpression(), ActivityType.Print);

            return new QxPrintStatement(expression);
        }

        private QxIfStatement ParseIf()
        {
            _parseContext.AssignStatementType(StatementType.If);

            var condition = CaptureExpression(() => ParseExpression(), ActivityType.IfCondition);

            Allow(TokenType.Then); // Allowed as syntactic sugar, but not necessary

            if (!Match(TokenType.NewLine))
            {
                if (Match(TokenType.Eof))
                    throw new IncompleteSourceException("End of file encountered before if block terminates.", GetDiagnostic(Issue.IncompleteSource()));

                _parseContext.BeginStatement();

                var statement = ParseStatement();

                _parseContext.AttachStatement(statement);

                _parseContext.EndStatement();

                return new QxIfStatement(condition) { ThenBlock = [statement] };
            }

            var thenBlock = CaptureActivity(() => ParseBlock(BlockType.If), ActivityType.IfThenBlock);

            List<QxElseIfClause> elseIfClauses = [];

            Block? elseBlock = null;

            while (Match(TokenType.Else, out var token))
            {
                if (elseBlock is not null)
                    throw new UnexpectedTokenException(token, TokenType.EndIf, GetDiagnostic(Issue.UnexpectedToken(token, TokenType.EndIf)));

                if (Match(TokenType.If))
                {
                    var elseIfClasue = ParseElseIfClause();
                    elseIfClauses.Add(elseIfClasue);
                }
                else
                {
                    elseBlock = CaptureActivity(() => ParseBlock(BlockType.Else), ActivityType.ElseBlock);
                }
            }

            Expect(TokenType.EndIf);

            return new QxIfStatement(condition)
            {
                ThenBlock = thenBlock,
                ElseIfClauses = elseIfClauses,
                ElseBlock = elseBlock ?? Block.Empty,
            };
        }

        private QxElseIfClause ParseElseIfClause()
        {
            var condition = CaptureExpression(() => ParseExpression(), ActivityType.ElseIfCondition);

            var block = CaptureActivity(() => ParseBlock(BlockType.ElseIf), ActivityType.ElseIfBlock);

            return new QxElseIfClause(condition)
            {
                Block = block,
            };
        }

        private QxStatement ParseFor()
        {
            _parseContext.AssignStatementType(StatementType.For);

            var iteratorToken = Expect(TokenType.Identifier);
            var iterator = iteratorToken.Value;

            if (IsToken(TokenType.In))
                return ParseForIn(iterator);

            // From value
            var from = CaptureExpression(() =>
            {
                Expect(TokenType.Assignment);

                return ParseExpression();

            }, ActivityType.FromValue);


            // To value
            var to = CaptureExpression(() =>
            {
                Expect(TokenType.To);

                return ParseExpression();
            }, ActivityType.ToValue);

            // Step value if present.
            var step = CaptureExpression(() =>
            {
                if (Match(TokenType.Step))
                    return ParseExpression();
                else
                    return new QxNumberLiteralExpression(1);

            }, ActivityType.StepValue);

            var block = CaptureActivity(() => ParseBlock(BlockType.For), ActivityType.ForBlock);

            Expect(TokenType.Next);

            return new QxForStatement(iterator, from, to)
            {
                Step = step,
                Block = block,
            };
        }

        private QxForInStatement ParseForIn(string iterator)
        {
            // From value
            var collection = CaptureExpression(() =>
            {
                Expect(TokenType.In);

                return ParseExpression();

            }, ActivityType.FromValue);

            var block = CaptureActivity(() => ParseBlock(BlockType.For), ActivityType.ForBlock);

            Expect(TokenType.Next);

            return new QxForInStatement(iterator, collection)
            {
                Block = block,
            };
        }

        private QxDoStatement ParseDo()
        {
            _parseContext.AssignStatementType(StatementType.Do);

            var isEntryControlled = false;
            QxExpression? condition = null;
            if (Match(TokenType.While))
            {
                isEntryControlled = true;
                condition = CaptureExpression(() => ParseExpression(), ActivityType.DoPrecondition);
            }
            else if (Match(TokenType.Until))
            {
                isEntryControlled = true;
                condition = CaptureExpression(() => ParseExpression(), ActivityType.DoPrecondition);
                condition = new QxUnaryExpression(Operator.Not, condition);
            }

            var block = CaptureActivity(() => ParseBlock(BlockType.Do), ActivityType.DoBlock);

            Expect(TokenType.Loop);

            if (Match(TokenType.While))
            {
                if (condition is not null)
                    throw new DoLoopDualConditionException(GetDiagnostic(Issue.DoLoopDualCondition()));

                isEntryControlled = false;
                condition = CaptureExpression(() => ParseExpression(), ActivityType.DoPostcondition);
            }
            else if (Match(TokenType.Until))
            {
                if (condition is not null)
                    throw new DoLoopDualConditionException(GetDiagnostic(Issue.DoLoopDualCondition()));

                isEntryControlled = false;
                condition = CaptureExpression(() => ParseExpression(), ActivityType.DoPostcondition);
                condition = new QxUnaryExpression(Operator.Not, condition);
            }

            if (condition is null)
                throw new DoLoopNoConditionException(GetDiagnostic(Issue.DoLoopNoCondition()));

            return new(condition, isEntryControlled)
            {
                Block = [.. block],
            };
        }

        private QxExpression ParseExpression(int parentPrecedence = 0)
        {
            var left = ParseUnary();

            return ParseExpression(left, parentPrecedence);
        }

        private QxExpression ParseExpression(QxExpression left, int parentPrecedence = 0)
        {

            while (true)
            {
                var token = Peek();

                QxExpression right;

                var operationMetadata = OperationMetadata.Get(OperationType.Binary, token.Type);

                var nextPrecedence = operationMetadata.Precedence;

                if (operationMetadata.Operator == Operator.None)
                    break;

                if (operationMetadata.Associativity == Associativity.Left)
                    nextPrecedence++;

                if (nextPrecedence <= parentPrecedence)
                    break;

                Advance(); // consume operator

                if (operationMetadata.Operator == Operator.Dot)
                {
                    left = CaptureExpression(() => ParseMethodCallExpression(left), ActivityType.MethodCall);
                    continue;
                }
                if (operationMetadata.Operator == Operator.Is)
                {
                    left = CaptureExpression(() => ParseIsComparison(left), ActivityType.IsComparison);
                    continue;
                }
                else
                {
                    right = CaptureExpression(() => ParseExpression(operationMetadata.Precedence), ActivityType.RightOperand);
                }


                switch (operationMetadata.Operator)
                {
                    case Operator.Indexer:
                        Expect(TokenType.CloseBracket);
                        left = new QxIndexerExpression(left, right);
                        break;

                    default:
                        left = new QxBinaryExpression(operationMetadata.Operator, left, right);
                        break;
                }
            }

            return left;
        }

        private QxExpression ParsePrimary()
        {
            var token = Peek();

            if (token.Type == TokenType.OpenParen)
            {
                return CaptureExpression(() =>
                {
                    Advance();
                    var expression = ParseExpression();

                    Expect(TokenType.CloseParen);

                    return expression;
                }, ActivityType.ParenSet);
            }

            if (token.Type == TokenType.OpenBracket)
            {
                return CaptureExpression(() =>
                {
                    Advance();
                    var expression = ParseArray();
                    Expect(TokenType.CloseBracket);
                    return expression;

                }, ActivityType.BracketSet);
            }

            if (token.Type == TokenType.OpenBrace)
            {
                return CaptureExpression(() =>
                {
                    Advance();
                    var expression = ParseSet();
                    Expect(TokenType.CloseBrace);
                    return expression;
                }, ActivityType.BraceSet);
            }

            if (token.Type == TokenType.StringLiteral)
            {
                return CaptureExpression(() =>
                {
                    Advance();
                    return new QxStringLiteralExpression(token.Value);
                }, ActivityType.StringLiteral);
            }

            if (token.Type == TokenType.NumberLiteral)
                return ParseNumber();

            if (token.Type == TokenType.True || token.Type == TokenType.False)
            {
                return CaptureExpression(() =>
                {
                    Advance();
                    return new QxBooleanLiteralExpression(string.Equals(token.Value, "true", StringComparison.OrdinalIgnoreCase));
                }, ActivityType.BooleanLiteral);
            }

            if (token.Type == TokenType.Identifier)
                return ParseIdentifierExpression();

            Advance(); // Consume current token
            throw new UnexpectedTokenException(token, GetDiagnostic(Issue.UnexpectedToken(token)));
        }

        private QxExpression ParseIdentifierExpression()
        {
            return CaptureExpression<QxExpression>(() =>
            {
                var name = ParseIdentifierName();

                if (Match(TokenType.OpenParen))
                    return ParseFunctionCallExpression(name);

                return new QxIdentifierExpression(name);
            }, ActivityType.Identifier);
        }

        private string ParseIdentifierName()
        {
            var token = Peek();
            Advance();

            return token.Value;
        }

        private string ParseTypeName()
        {
            var typeName = ParseIdentifierName();

            if (Match(TokenType.OpenBracket))
            {
                Expect(TokenType.CloseBracket);
                typeName += "[]";
            }

            return typeName;
        }

        private QxIsComparisonExpression ParseIsComparison(QxExpression instance)
        {
            var typeName = ParseTypeName();

            string? patternIdentifier = Match(TokenType.Identifier, out var token) ? token.Value : null;

            return new(instance, typeName, patternIdentifier);
        }

        private QxExpression ParseUnary()
        {
            if (Match(TokenType.Plus))
                return CaptureExpression(() => new QxUnaryExpression(Operator.Add, ParseUnary()), ActivityType.UnaryPlus);

            if (Match(TokenType.Subtract))
                return CaptureExpression(() => new QxUnaryExpression(Operator.Subtract, ParseUnary()), ActivityType.UnaryNegation);

            if (Match(TokenType.Not))
                return CaptureExpression(() => new QxUnaryExpression(Operator.Not, ParseUnary()), ActivityType.UnaryNot);

            if (Match(TokenType.New))
                return CaptureExpression(() => new QxUnaryExpression(Operator.New, ParseTypeInstantiation()), ActivityType.Instantiation);

            return ParsePrimary();
        }

        private Block ParseBlock(BlockType blockType)
        {
            var block = new Block();

            List<TokenType> terminatingTypes = blockType switch
            {
                BlockType.If => [TokenType.Else, TokenType.EndIf],
                BlockType.ElseIf => [TokenType.Else, TokenType.EndIf],
                BlockType.Else => [TokenType.Else, TokenType.EndIf],
                BlockType.Do => [TokenType.Loop],
                BlockType.For => [TokenType.Next],
                BlockType.Function => [TokenType.EndFunction],
                BlockType.Constructor => [TokenType.EndConstruct],
                BlockType.Type => [TokenType.EndType],
                _ => []
            };

            bool isTerminated()
            {
                if (IsAtEnd || Match(TokenType.Eof))
                    throw new IncompleteSourceException("Encountered end of file before block is terminated.", GetDiagnostic(Issue.IncompleteSource()));

                return terminatingTypes.Any(t => Peek().Type == t);
            }

            while (!isTerminated())
            {
                ConsumeNewLines();

                if (IsAtEnd || Match(TokenType.Eof))
                    throw new IncompleteSourceException("Encountered end of file before block is terminated.", GetDiagnostic(Issue.IncompleteSource()));

                if (isTerminated())
                    break;

                _parseContext.BeginStatement();

                var statement = ParseStatement();

                _parseContext.AttachStatement(statement);

                block.Add(statement);

                _parseContext.EndStatement();

                ConsumeStatementTerminator();
            }

            return block;
        }

        private Block ParseDeclarationBlock()
        {
            bool isTerminated()
            {
                if (IsAtEnd || Match(TokenType.Eof))
                    throw new IncompleteSourceException("Encountered end of file before block is terminated.", GetDiagnostic(Issue.IncompleteSource()));

                if (IsToken(TokenType.EndType))
                    return true;

                return false;
            }

            var block = new Block();

            while (!isTerminated())
            {
                ConsumeNewLines();

                if (IsAtEnd || Match(TokenType.Eof))
                    throw new IncompleteSourceException("Encountered end of file before block is terminated.", GetDiagnostic(Issue.IncompleteSource()));

                if (isTerminated())
                    break;

                _parseContext.BeginStatement();

                var next = Peek();

                var statement = ParseDeclarationStatement();

                _parseContext.AttachStatement(statement);

                block.Add(statement);

                _parseContext.EndStatement();

                ConsumeStatementTerminator();
            }

            return block;
        }

        private QxNumberLiteralExpression ParseNumber()
        {
            return CaptureExpression(() =>
            {
                var token = Peek();
                Advance();
                if (double.TryParse(token.Value, out var number))
                    return new QxNumberLiteralExpression(number);

                throw new TokenException("Invalid number literal", token, GetDiagnostic(Issue.InvalidNumber()));
            }, ActivityType.NumberLiteral);
        }

        private TValue CaptureActivity<TValue>(Func<TValue> activity, ActivityType expressionType)
        {
            _parseContext.BeginActivity(expressionType);

            var value = activity();

            _parseContext.EndActivity();

            return value;
        }

        private void CaptureActivity(Action activity, ActivityType expressionType)
        {
            _parseContext.BeginActivity(expressionType);

            activity();

            _parseContext.EndActivity();
        }

        private TExpression CaptureExpression<TExpression>(Func<TExpression> expressionFactory, ActivityType expressionType)
            where TExpression : QxExpression
        {
            _parseContext.BeginActivity(expressionType);

            var expression = expressionFactory();

            _parseContext.AttachExpression(expression);

            _parseContext.EndActivity();

            return expression;
        }

        private Diagnostic GetDiagnostic(Issue issue) => _parseContext.GetDiagnostic(issue);

        private bool IsAtEnd => _tokens.IsAtEnd;

        private Token Peek() => _tokens.Peek();

        private bool Advance()
        {
            _parseContext.ConsumeToken(Peek());
            var canAdvance = _tokens.Advance();

#if DEBUG
            if (Current is not null)
            {
                _previous.Enqueue(Current);

                while (_previous.Count > _previousCount)
                    _previous.Dequeue();
            }

            Current = IsAtEnd ? null : Peek();
#endif

            return canAdvance;
        }

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

            //if (Match(TokenType.Eof))
            //    throw new IncompleteSourceException($"Encountered end of file before expected {type}.", GetDiagnostic(Issue.IncompleteSource()));

            var nextToken = Peek();
            Advance();
            throw new UnexpectedTokenException(nextToken, type, GetDiagnostic(Issue.UnexpectedToken(nextToken, type)));
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

        /// <summary>
        /// Match next token without advancing.
        /// </summary>
        private bool IsToken(TokenType type)
        {
            if (IsAtEnd)
                return false;

            return Peek().Type == type;
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

        public static IEnumerable<QxStatement> Parse(string source)
        {
            var lexer = new Lexer(source);
            var parser = new Parser(lexer);
            return parser.Parse();
        }

    }
}
