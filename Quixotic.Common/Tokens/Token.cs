namespace Quixotic.Common.Tokens
{

    public class Token : TokenHead
    {
        public required Span Span { get; init; }

        public override string ToString() => $"{Type}('{Value}') ({Span.Start.Line}:{Span.Start.Column})";
    }
}
