using Quixotic.Common.Contracts;
using Quixotic.Common.Source;
using Quixotic.Common.Statements;

namespace Quixotic.Analysis.Sessions
{
    public class Session(ISource source, Block root)
    {
        public Session(string source, Block root)
            : this(new StringSource(source), root)
        { }

        public ISource Source { get; } = source;

        public SourceDatabase SourceDatabase { get; set; } = new(source);

        public Block Root { get; } = root;
    }
}
