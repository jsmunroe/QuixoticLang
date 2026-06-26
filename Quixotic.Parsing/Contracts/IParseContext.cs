using Quixotic.Common.Diagnostics;
using Quixotic.Common.Diagnostics.Issues;
using Quixotic.Common.Expressions;
using Quixotic.Common.Statements;
using Quixotic.Common.Tokens;

namespace Quixotic.Parsing.Context
{
    public interface IParseContext
    {
        ActivityContext? CurrentActivity { get; }
        StatementContext? CurrentStatement { get; }

        void ConsumeToken(Token token);

        void BeginStatement();
        void AttachStatement(QxStatement statement);
        void EndStatement();
        void BeginActivity(ActivityType expressionType);
        void AttachExpression(QxExpression expression);
        void EndActivity();
        void AssignStatementType(StatementType type);

        Diagnostic GetDiagnostic(Issue issue);
    }
}