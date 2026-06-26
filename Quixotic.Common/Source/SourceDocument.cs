using Quixotic.Common.Contracts;
using Quixotic.Common.Tokens;
using System.Text.RegularExpressions;

namespace Quixotic.Common.Source
{
    public class SourceDocument
    {
        private readonly List<SourceLine> _lines;

        private static readonly Regex _rexLineStarts = new(@"[^\r\n]*(?:\r?\n|$)", RegexOptions.Compiled);

        public SourceDocument(ISource source)
            : this(source.GetFullText())
        { }

        public SourceDocument(string source)
        {
            SourceText = source;
            _lines = ComputeSourceLines();
        }

        public string SourceText { get; }

        public IReadOnlyList<SourceLine> Lines => _lines.AsReadOnly();

        public int Length => SourceText.Length;

        public string? GetText(int start, int length)
        {
            return SourceText.Substring(start, length);
        }

        public string? GetText(Span span)
        {
            return GetText(span.Start.Index, span.End.Index);
        }

        public SourceLine? GetLineFromIndex(int index)
        {
            int low = 0;
            int high = _lines.Count - 1;

            while (low <= high)
            {
                int mid = (low + high) / 2;

                var line = _lines[mid];

                if (index < line.Start)
                    high = mid - 1;
                else if (index > line.End)
                    low = mid + 1;
                else
                    return line;
            }

            return null;
        }

        public SourceLine? GetLine(int lineNumber)
        {
            if (lineNumber < 1)
                return null;

            if (lineNumber > _lines.Count)
                return null;

            return _lines[lineNumber - 1];
        }

        public int GetLineNumber(int index)
        {
            var line = GetLineFromIndex(index);

            return line?.LineNumber ?? -1;
        }

        public int GetColumnNumber(int index)
        {
            var line = GetLineFromIndex(index);

            if (line is null)
                return -1;

            return index - line.Start + 1;
        }

        public int GetIndex(int lineNumber, int columnNumber)
        {
            var line = GetLine(lineNumber);

            if (line is null)
                return -1;

            var index = line.Start + columnNumber - 1;

            if (index > line.End)
                return -1;

            return index;
        }

        public Position GetPosition(int index)
        {
            var line = GetLineNumber(index);
            var column = GetColumnNumber(index);

            if (line < 0 || column < 0)
                return Position.Empty;

            return new Position
            {
                Index = index,
                Line = line,
                Column = column,
            };
        }

        private List<SourceLine> ComputeSourceLines()
        {
            if (SourceText == string.Empty)
            {
                return [
                        new SourceLine(this) {
                        LineNumber = 1,
                        Start = 0,
                        Length = 0,
                    }
                ];
            }

            var matches = _rexLineStarts.Matches(SourceText);

            var lines = matches.Cast<Match>().Select((match, index) =>
            {
                return new SourceLine(this)
                {
                    LineNumber = index + 1,
                    Start = match.Index,
                    Length = match.Length,
                };

            }).Where(l => l.Length > 0); // Removes only the trailing empty match. The rest of the lines will have at least a new line character.

            return [.. lines];
        }
    }
}
