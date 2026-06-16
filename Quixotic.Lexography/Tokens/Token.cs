namespace Quixotic.Lexography.Tokens
{
    public class Token
    {
        public required TokenType Type { get; init; }
        public required string Value { get; init; }
        public required Position Position { get; init; }

        public override string ToString() => $"{Type}('{Value}') ({Position.Line}:{Position.Column})";
    }
}
