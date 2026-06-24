using Quixotic.Common.Tokens;

namespace Quixotic.Common.Exceptions.Lexography
{
    public class LexerUnexpectedCharacterException(char character, Position position)
        : LexerException($"Unexpected character '{character}' ({position.Line}:{position.Column})", position)
    {
        public char Character { get; } = character;
    }
}
