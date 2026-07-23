using Quixotic.Common.TypeSystem.Types;

namespace Quixotic.Common.Analysis.Statements
{
    public class TypeDeclarationStatementInfo : StatementInfo
    {
        public required QxType Type { get; init; }

        public required QxType? BaseType { get; init; }

        public IReadOnlyList<StatementInfo> MemberStatements { get; init; } = [];
    }
}
