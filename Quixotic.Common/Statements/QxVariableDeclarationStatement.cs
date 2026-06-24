using Quixotic.Common.Expressions;

namespace Quixotic.Common.Statements
{
    public class QxVariableDeclarationStatement(string name, QxExpression? valueExpression) : QxStatement
    {
        public string Name { get; } = name;

        public QxExpression? Value { get; } = valueExpression;
    }
}
