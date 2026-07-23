using Quixotic.Common.Statements;
using Quixotic.Common.TypeSystem.Types;

namespace Quixotic.Common.Analysis.Statements
{
    public class TypeDeclarationStatementInfo(QxStatement statement) : StatementInfo(statement)
    {
        public required QxType Type { get; init; }

        public required QxType? BaseType { get; init; }

        public IReadOnlyList<StatementInfo> MemberStatements { get; init; } = [];
    }
}
