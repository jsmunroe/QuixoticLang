using Quixotic.Common.Tokens;

namespace Quixotic.Common.Exceptions.Lexography
{
    public class LexerException(string message, Position position) : Exception(message)
    {
        public Position Position { get; } = position;
    }
}
