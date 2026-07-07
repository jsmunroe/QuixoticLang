namespace Quixotic.Common.Expressions
{
    public class QxMethodCallExpression(QxExpression target, string methodName) : QxExpression
    {
        public QxExpression Target { get; } = target;

        public string MethodName { get; } = methodName;

        public List<QxExpression> Arguments { get; init; } = [];
    }
}
