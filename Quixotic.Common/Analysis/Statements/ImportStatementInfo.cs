using Quixotic.Common.Namespaces;

namespace Quixotic.Common.Analysis.Statements
{
    public class ImportStatementInfo : StatementInfo
    {
        public required Namespace Namespace { get; init; }
    }
}
