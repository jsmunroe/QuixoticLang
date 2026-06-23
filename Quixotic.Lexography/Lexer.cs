using Quixotic.Lexography.Exceptions;
using Quixotic.Lexography.Tokens;

namespace QuixoticLang.Lexer
{
    public class Lexer(TextReader reader)
    {
        private int _index;
        private int _line = 1;
        private int _column = 1;

        public Lexer(string source)
            : this(new StringReader(source))
        { }

        public Lexer(Stream stream)
            : this(new StreamReader(stream))
        { }

        public List<Token> Tokens { get; } = [];

        private static Dictionary<string, TokenType> Keywords { get; } = new(StringComparer.OrdinalIgnoreCase)
        {
            { "print", TokenType.Print },
            { "let", TokenType.Let },
            { "if", TokenType.If },
            { "then", TokenType.Then },
            { "else", TokenType.Else },
            { "for", TokenType.For },
            { "to", TokenType.To },
            { "step", TokenType.Step },
            { "next", TokenType.Next },
            { "do", TokenType.Do },
            { "while", TokenType.While },
            { "until", TokenType.Until },
            { "loop", TokenType.Loop },
            { "continue", TokenType.Continue },
            { "break", TokenType.Break },
            { "end", TokenType.End },
            { "true", TokenType.BooleanLiteral },
            { "false", TokenType.BooleanLiteral },
            { "and", TokenType.And },
            { "or", TokenType.Or },
            { "not", TokenType.Not },
            { "function", TokenType.Function },
            { "return", TokenType.Return },
        };

        public IEnumerable<Token> Tokenize()
        {
            Tokens.Clear();
            foreach (var token in TokenizeInternal())
            {
                Tokens.Add(token);
                yield return token;
            }
        }

        private IEnumerable<Token> TokenizeInternal()
        {
            while (!IsAtEnd())
            {
                var c = Peek();

                if (c == '\0') // At end of input.
                    break;


                if (c == '\n')
                {
                    yield return Simple(TokenType.NewLine, "\n");
                    Advance();
                    continue;
                }

                if (char.IsWhiteSpace(c))
                {
                    ConsumeWhitespace();
                    continue;
                }

                if (char.IsLetter(c))
                {
                    var token = ReadIdentifierOrKeyword();
                    yield return token;
                    continue;
                }

                if (char.IsNumber(c))
                {
                    var token = ReadNumber();
                    yield return token;
                    continue;
                }

                if (c == '"')
                {
                    var token = ReadString();
                    yield return token;
                    continue;
                }

                // single-character tokens
                switch (c)
                {
                    case '(':
                        yield return Simple(TokenType.OpenParen, "(");
                        Advance();
                        break;

                    case ')':
                        yield return Simple(TokenType.CloseParen, ")");
                        Advance();
                        break;

                    case '=':
                        yield return Simple(TokenType.EqualTo, "=");
                        Advance();
                        break;

                    case '>':
                        yield return ReadGreaterThanOperator();
                        break;

                    case '<':
                        yield return ReadLessThanOperator();
                        break;

                    case '!':
                        yield return ReadExclamationPoint();
                        break;

                    case ':':
                        yield return ReadAssignmentOperator();
                        break;

                    case '-':
                        yield return Simple(TokenType.Subtract, "-");
                        Advance();
                        break;

                    case '+':
                        yield return Simple(TokenType.Plus, "+");
                        Advance();
                        break;

                    case '/':
                        yield return Simple(TokenType.Divide, "/");
                        Advance();
                        break;

                    case '*':
                        yield return Simple(TokenType.Multiply, "*");
                        Advance();
                        break;

                    case ',':
                        yield return Simple(TokenType.Comma, ",");
                        Advance();
                        break;

                    default:
                        throw new LexerUnexpectedCharacterException(c, CurrentPosition());
                }
            }

            yield return Eof();
        }

        private char Peek(int offset = 0)
        {
            var next = reader.Peek();

            return next == -1 ? '\0' : (char)next;
        }

        private char Advance()
        {
            var next = reader.Read();
            var nextChar = next == -1 ? '\0' : (char)next;

            _index++;
            _column++;

            if (nextChar == '\n')
            {
                _line++;
                _column = 1;
            }

            return nextChar;
        }

        private bool IsAtEnd() => reader.Peek() == -1;

        private Position CurrentPosition() => new()
        {
            Line = _line,
            Column = _column,
            Index = _index
        };

        private void ConsumeWhitespace()
        {
            var c = Peek();
            while (!IsAtEnd() && char.IsWhiteSpace(c) && c != '\n')
            {
                Advance();

                c = Peek();
            }
        }

        private Token ReadIdentifierOrKeyword()
        {
            var start = _index;
            var position = CurrentPosition();

            string text = string.Empty;
            var next = Peek();
            while (!IsAtEnd() && char.IsLetter(next))
            {
                text += next;
                Advance();
                next = Peek();
            }

            if (!Keywords.TryGetValue(text, out var tokenType))
                tokenType = TokenType.Identifier;

            return new Token
            {
                Type = tokenType,
                Value = text,
                Position = position,
            };
        }

        private Token ReadNumber()
        {
            var position = CurrentPosition();

            int start = _index;

            bool hasDot = false;
            bool hasExponent = false;

            var number = string.Empty;
            while (!IsAtEnd())
            {
                char c = Peek();

                if (char.IsDigit(c))
                {
                    number += c;
                    Advance();
                    continue;
                }

                if (c == '.' && !hasDot && !hasExponent)
                {
                    number += c;
                    hasDot = true;
                    Advance();
                    continue;
                }

                if ((c == 'e' || c == 'E') && !hasExponent)
                {
                    number += c;
                    hasExponent = true;
                    Advance();

                    // Handle optional exponent sign
                    if (!IsAtEnd() && (Peek() == '+' || Peek() == '-'))
                        Advance();

                    continue;
                }

                break;
            }

            return new Token
            {
                Type = TokenType.NumberLiteral,
                Value = number,
                Position = position
            };
        }

        private Token ReadString()
        {
            var position = CurrentPosition();
            Advance(); // skip opening quote (")

            int start = _index;

            var stringValue = string.Empty;

            while (!IsAtEnd() && Peek() != '"')
            {
                if (Peek() == '\\')  // Handle escape quotes
                    stringValue += Advance();

                stringValue += Advance();
            }

            stringValue = Unescape(stringValue);

            Advance(); // skip closing quote (")

            return new Token
            {
                Type = TokenType.StringLiteral,
                Value = stringValue,
                Position = position
            };
        }

        private Token ReadAssignmentOperator()
        {
            var position = CurrentPosition();

            Advance();

            var c = Peek();

            if (c == '=')
            {
                Advance();
                return Simple(TokenType.Assignment, ":=", position);
            }

            throw new LexerUnexpectedCharacterException(c, position);
        }

        private Token ReadGreaterThanOperator()
        {
            var position = CurrentPosition();

            Advance();

            var c = Peek();

            if (c == '=')
            {
                Advance();
                return Simple(TokenType.GreaterThanOrEqualTo, ">=", position);
            }

            return Simple(TokenType.GreaterThan, ">", position);
        }

        private Token ReadLessThanOperator()
        {
            var position = CurrentPosition();

            Advance();

            var c = Peek();

            if (c == '=')
            {
                Advance();
                return Simple(TokenType.LessThanOrEqualTo, "<=", position);
            }

            return Simple(TokenType.LessThan, "<", position);
        }

        private Token ReadExclamationPoint()
        {
            var position = CurrentPosition();

            Advance();

            var c = Peek();

            if (c == '=')
            {
                Advance();
                return Simple(TokenType.NotEqualTo, "!=", position);
            }

            throw new LexerUnexpectedCharacterException(c, position);
        }

        private Token Simple(TokenType type, string? value, Position? position = null)
        {
            return new()
            {
                Type = type,
                Value = value ?? string.Empty,
                Position = position ?? CurrentPosition(),
            };
        }

        private static string Unescape(string value)
        {
            return value.Replace("\\\"", "\"");
        }

        private Token Eof()
        {
            return new()
            {
                Type = TokenType.Eof,
                Value = string.Empty,
                Position = CurrentPosition(),
            };
        }
    }
}
