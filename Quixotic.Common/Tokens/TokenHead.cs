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

        public override string ToString() => $"{Type}('{Value}')";
    }
}
