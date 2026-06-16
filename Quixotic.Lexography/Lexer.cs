using Quixotic.Lexography.Exceptions;
using Quixotic.Lexography.Tokens;

namespace QuixoticLang.Lexer
{
    public class Lexer(string source)
    {
        private readonly string _source = source;
        private int _index;
        private int _line = 1;
        private int _column = 1;

        private static Dictionary<string, TokenType> Keywords { get; } = new(StringComparer.OrdinalIgnoreCase)
        {
            { "print", TokenType.Print },
            { "if", TokenType.If },
            { "else", TokenType.Else },
            { "end", TokenType.End },
        };

        private readonly IEqualityComparer<string> _keywordComparer = StringComparer.OrdinalIgnoreCase;

        public IEnumerable<Token> Run()
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
                        yield return Simple(TokenType.LeftParen, "(");
                        Advance();
                        break;

                    case ')':
                        yield return Simple(TokenType.RightParen, ")");
                        Advance();
                        break;

                    case '=':
                        yield return Simple(TokenType.Equals, "=");
                        Advance();
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

                    default:
                        throw new LexerUnexpectedCharacterException(c, CurrentPosition());
                }
            }

            yield return Eof();
        }

        private char Peek(int offset = 0)
        {
            if (_index + offset >= _source.Length)
                return '\0'; // null char to indicate end of input

            return _source[_index + offset];
        }

        private char Advance()
        {
            if (_index >= _source.Length)
                return '\0'; // null char to indicate end of input

            char c = _source[_index++];
            _column++;

            if (c == '\n')
            {
                _line++;
                _column = 1;
            }

            return c;
        }

        private bool IsAtEnd() => _index >= _source.Length;

        private Position CurrentPosition() => new()
        {
            Line = _line,
            Column = _column,
            Index = _index
        };

        private void ConsumeWhitespace()
        {
            while (!IsAtEnd() && char.IsWhiteSpace(Peek()))
            {
                Advance();
            }
        }

        private Token ReadIdentifierOrKeyword()
        {
            var start = _index;
            var position = CurrentPosition();

            while (!IsAtEnd() && char.IsLetter(Peek()))
                Advance();

            var text = _source[start.._index];

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

            while (!IsAtEnd())
            {
                char c = Peek();

                if (char.IsDigit(c))
                {
                    Advance();
                    continue;
                }

                if (c == '.' && !hasDot && !hasExponent)
                {
                    hasDot = true;
                    Advance();
                    continue;
                }

                if ((c == 'e' || c == 'E') && !hasExponent)
                {
                    hasExponent = true;
                    Advance();

                    // Handle optional exponent sign
                    if (!IsAtEnd() && Peek() == '+' || Peek() == '-')
                        Advance();

                    continue;
                }

                break;
            }

            var value = _source[start.._index];

            return new Token
            {
                Type = TokenType.NumberLiteral,
                Value = value,
                Position = position
            };
        }

        private Token ReadString()
        {
            var position = CurrentPosition();
            Advance(); // skip opening quote (")

            int start = _index;

            while (!IsAtEnd() && Peek() != '"')
            {
                if (Peek() == '\\' && Peek(1) == '"') // Handle escape quotes
                    Advance();

                Advance();
            }

            var value = _source[start.._index];

            value = Unescape(value);

            Advance(); // skip closing quote (")

            return new Token
            {
                Type = TokenType.StringLiteral,
                Value = value,
                Position = position
            };
        }

        private Token Simple(TokenType type, string? value)
        {
            return new()
            {
                Type = type,
                Value = value ?? string.Empty,
                Position = CurrentPosition(),
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
