using Quixotic.Common.Tokens;

namespace Quixotic.Common.Source
{
    public class SourceLine(SourceDocument document)
    {
        public SourceDocument Document { get; } = document;
        public int LineNumber { get; set; }

        public int Start { get; init; }
        public int Length { get; init; }

        public int End => Start + Length - 1;

        public Position StartPosition => Document.GetPosition(Start);

        public Position EndPosition => Document.GetPosition(End);

        public string Text => Document.GetText(Start, Length);
    }
}
