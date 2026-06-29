using Quixotic.Common.Expressions;

namespace Quixotic.Common.Statements
{
    public class QxVariableDeclarationStatement(string name, string? typeName, QxExpression? valueExpression) : QxStatement
    {
        public string Name { get; } = name;

        public string? TypeName { get; } = typeName;

        public QxExpression? Value { get; } = valueExpression;

        public QxVariableDeclarationStatement(string name, QxExpression? valueExpression)
            : this(name, null, valueExpression)
        { }
    }
}
