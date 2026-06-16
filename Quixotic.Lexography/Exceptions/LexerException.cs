using Quixotic.Lexography.Tokens;

namespace Quixotic.Lexography.Exceptions
{
    public class LexerException(string message, Position position) : Exception(message)
    {
        public Position Position { get; } = position;
    }
}
