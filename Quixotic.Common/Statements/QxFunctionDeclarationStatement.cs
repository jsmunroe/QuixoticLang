using Quixotic.Common.Expressions;

namespace Quixotic.Common.Statements
{

    public class QxFunctionDeclarationStatement(string name, QxFunctionExpression expression, bool isMember) : QxStatement
    {
        public string Name { get; } = name;
        public bool IsMember { get; } = isMember;

        public QxFunctionExpression Expression { get; } = expression;

    }
}
