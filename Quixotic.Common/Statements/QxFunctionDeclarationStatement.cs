using Quixotic.Common.Expressions;

namespace Quixotic.Common.Statements
{

    public class QxFunctionDeclarationStatement(string name, QxFunctionExpression expression) : QxStatement
    {
        public string Name { get; } = name;

        public QxFunctionExpression Expression { get; } = expression;
    }
}
