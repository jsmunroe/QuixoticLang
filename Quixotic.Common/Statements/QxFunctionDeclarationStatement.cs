using Quixotic.Common.Expressions;

namespace Quixotic.Common.Statements
{

    public class QxFunctionDeclarationStatement(string name, string returnType) : QxStatement
    {
        public string Name { get; } = name;

        public string ReturnType { get; } = returnType;

        public List<QxParameter> Parameters { get; init; } = [];

        public Block Body { get; init; } = [];
    }

    public class QxConstructorDeclarationStatement(QxBaseConstructorCallExpression? baseCall) : QxStatement
    {
        public QxBaseConstructorCallExpression? BaseCall { get; } = baseCall;

        public List<QxParameter> Parameters { get; init; } = [];

        public Block Body { get; init; } = [];

    }
}
