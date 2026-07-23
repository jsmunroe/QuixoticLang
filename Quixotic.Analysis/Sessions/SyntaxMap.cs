using Quixotic.Common.Statements;

namespace Quixotic.Analysis.Sessions
{
    public class SyntaxMap(Block root)
    {
        public Block Root { get; } = root;
    }
}
