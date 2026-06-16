using Quixotic.Lexography.Tokens;

namespace Quixotic.Lexography.Exceptions
{
    public class LexerUnexpectedCharacterException(char character, Position position)
        : LexerException($"Unexpected character '{character}' ({position.Line}:{position.Column})", position)
    {
        public char Character { get; } = character;
    }
}
