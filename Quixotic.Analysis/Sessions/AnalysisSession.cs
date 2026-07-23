using Quixotic.Analysis.Exceptions;
using Quixotic.Common.Analysis;
using Quixotic.Common.Contracts;
using Quixotic.Common.Source;
using Quixotic.Common.Statements;

namespace Quixotic.Analysis.Sessions
{
    public class AnalysisSession(ISource source, Block root)
    {
        public AnalysisSession(string source, Block root)
            : this(new StringSource(source), root)
        { }

        public ISource Source { get; } = source;

        public List<SemanticException> Issues { get; } = [];

        public SourceDatabase SourceDatabase { get; set; } = new(source);

        public Block AstRoot { get; } = root;

        public DocumentInfo? Document { get; set; }
    }
}
