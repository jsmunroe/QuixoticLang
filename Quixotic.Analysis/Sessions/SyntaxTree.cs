using Quixotic.Common.Statements;
using Quixotic.Common.Tokens;

namespace Quixotic.Analysis.Sessions
{
    public class SyntaxTree(Block root, List<Token> tokens)
    {
        public Block Root { get; } = root;

        public IReadOnlyList<Token> Tokens { get; } = tokens.AsReadOnly();
    }
}
