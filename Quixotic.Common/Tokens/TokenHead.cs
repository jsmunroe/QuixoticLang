using System.Diagnostics.CodeAnalysis;

namespace Quixotic.Common.Tokens
{
    public class TokenHead()
    {
        [SetsRequiredMembers]
        public TokenHead(TokenType type, string value)
            : this()
        {
            Type = type;
            Value = value;
        }

        public required TokenType Type { get; init; }
        public required string Value { get; init; }

        public bool IsEof => Type == TokenType.Eof;

        public bool IsNewLine => Type == TokenType.NewLine;

        public bool IsTerminator => Type == TokenType.NewLine || Type == TokenType.Eof;

        public override string ToString() => $"{Type}('{Value}')";
    }
}
