using Quixotic.Parsing.Expressions;

namespace Quixotic.Parsing.Statements
{
    public class QxVariableDeclarationStatement(string name, QxExpression? valueExpression) : QxStatement
    {
        public string Name { get; } = name;

        public QxExpression? Value { get; } = valueExpression;
    }
}
