namespace Quixotic.Common.Tokens
{
    public readonly struct Position
    {
        public int Line { get; init; }
        public int Column { get; init; }
        public int Index { get; init; }

        public bool IsEmpty => Index < 0;

        public static Position Empty { get; } = new Position { Line = 0, Column = 0, Index = -1 };

        public static Position operator +(Position left, int right)
        {
            return new Position
            {
                Index = left.Index + right,
                Line = left.Line,
                Column = left.Column + right,
            };
        }

    }
}
