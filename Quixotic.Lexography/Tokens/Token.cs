namespace Quixotic.Lexography.Tokens
{

    public class Token : TokenHead
    {
        public required Position Position { get; init; }

        public override string ToString() => $"{Type}('{Value}') ({Position.Line}:{Position.Column})";
    }
}
