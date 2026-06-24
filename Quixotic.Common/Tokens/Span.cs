namespace Quixotic.Common.Tokens
{
    public readonly struct Span
    {
        public Position Start { get; init; }
        public Position End { get; init; }

        public bool IsEmpty => Start.IsEmpty || End.IsEmpty;

        public static Span Empty { get; } = new Span { Start = Position.Empty, End = Position.Empty };

        public static Span operator +(Span left, Span right)
        {
            var start = left.Start.Index < right.Start.Index ? left.Start : right.Start;
            var end = left.End.Index > right.End.Index ? left.End : right.End;

            return new()
            {
                Start = start,
                End = end,
            };
        }
    }
}
