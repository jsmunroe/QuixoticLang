namespace Quixotic.Common.Tokens
{
    public readonly struct Span
    {
        public Position Start { get; init; }
        public Position End { get; init; }

        public bool IsEmpty => Start.IsEmpty || End.IsEmpty;

        public static Span Empty { get; } = new Span { Start = Position.Empty, End = Position.Empty };
    }
}
