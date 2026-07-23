using Quixotic.Common.Analysis.Expressions;
using Quixotic.Common.Symbols;
using Quixotic.Common.TypeSystem.Types;
using System.Diagnostics.CodeAnalysis;

namespace Quixotic.Common.Analysis.Statements
{
    public class VariableDeclarationStatementInfo : StatementInfo
    {
        public required string Name { get; init; }
        public required ExpressionInfo? Value { get; init; }
        public required QxType? DeclaredType { get; init; }

        [MemberNotNullWhen(true, nameof(MemberOf))]
        public bool IsPropertyMember { get; init; }
        public QxType? MemberOf { get; init; }

        public IdentifierSymbol? IdentifierSymbol { get; init; }

        public SignatureSymbol? GetterSymbol { get; init; }
        public SignatureSymbol? SetterSymbol { get; init; }

    }
}
