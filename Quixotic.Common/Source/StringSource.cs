using Quixotic.Common.Contracts;
using Quixotic.Common.Exceptions.Source;
using Quixotic.Common.Tokens;

namespace Quixotic.Common.Source
{
    public class StringSource(string source) : ISource
    {
        private int _index;
        private int _line = 1;
        private int _column = 1;

        public Position Position => new()
        {
            Index = _index,
            Line = _line,
            Column = _column,
        };

        public bool IsAtEnd => _index >= source.Length;

        public string GetFullText() => source;

        public char Peek()
        {
            return IsAtEnd ? '\0' : source[_index];
        }

        public char Advance()
        {
            var nextChar = Peek();

            _index++;
            _column++;

            if (nextChar == '\n')
            {
                _line++;
                _column = 1;
            }

            return nextChar;
        }

        public static ISource FromStream(Stream stream)
        {
            if (!stream.CanRead)
                throw new SourceException("Cannot read source from stream.");

            var reader = new StreamReader(stream);
            var source = reader.ReadToEnd();
            return new StringSource(source);
        }

        public static ISource FromFile(string filePath)
        {
            if (!File.Exists(filePath))
                throw new SourceException("Source file doesn't exist.");

            var source = File.ReadAllText(filePath);
            return new StringSource(source);
        }
    }
}
