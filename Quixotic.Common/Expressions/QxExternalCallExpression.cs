using Quixotic.Common.TypeSystem;

namespace Quixotic.Common.Expressions
{
    public class QxExternalCallExpression(Delegate call) : QxExpression
    {
        public List<QxExpression> Arguments { get; init; } = [];

        public Instance Invoke(params Instance[] arguments) => call.DynamicInvoke(arguments) as Instance ?? throw new InvalidOperationException("Invalid return type from external call.");
    }
}
