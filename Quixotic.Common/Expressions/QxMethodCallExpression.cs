using Quixotic.Common.Symbols;

namespace Quixotic.Common.Expressions
{
    public class QxMethodCallExpression(QxExpression target, string methodName, FunctionCallType type) : QxExpression
    {
        public QxExpression Target { get; } = target;

        public string MethodName { get; } = methodName;

        public FunctionCallType Type { get; } = type;

        public List<QxExpression> Arguments { get; init; } = [];
    }
}
