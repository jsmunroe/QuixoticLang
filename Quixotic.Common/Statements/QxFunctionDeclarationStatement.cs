using Quixotic.Common.Expressions;

namespace Quixotic.Common.Statements
{

    public class QxFunctionDeclarationStatement(string name, QxFunctionDeclarationExpression expression) : QxStatement
    {
        public string Name { get; } = name;

        public QxFunctionDeclarationExpression Expression { get; } = expression;
    }
}
