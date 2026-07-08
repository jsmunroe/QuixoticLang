using Quixotic.Common.Tokens;

namespace Quixotic.Common.Contracts
{
    public interface ISource
    {
        bool IsAtEnd { get; }
        Position Position { get; }

        char Advance();
        string GetFullText();
        char Peek(int offset = 0);
    }
}
