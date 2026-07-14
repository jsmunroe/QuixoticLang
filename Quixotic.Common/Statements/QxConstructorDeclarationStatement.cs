using Quixotic.Common.Expressions;

namespace Quixotic.Common.Statements
{
    public class QxConstructorDeclarationStatement(QxBaseConstructorCallExpression? baseCall) : QxStatement
    {
        public QxBaseConstructorCallExpression? BaseCall { get; } = baseCall;

        public List<QxParameter> Parameters { get; init; } = [];

        public Block Body { get; init; } = [];

    }
}
