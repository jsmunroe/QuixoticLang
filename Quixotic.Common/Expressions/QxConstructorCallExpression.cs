namespace Quixotic.Common.Expressions
{
    public class QxConstructorCallExpression(string typeName) : QxExpression
    {
        public string TypeName { get; } = typeName;

        public List<QxExpression> Arguments { get; init; } = [];
    }
}
