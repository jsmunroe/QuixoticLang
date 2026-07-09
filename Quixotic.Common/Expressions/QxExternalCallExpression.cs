using Quixotic.Common.TypeSystem;
using Quixotic.Common.TypeSystem.Types;

namespace Quixotic.Common.Expressions
{
    public class QxExternalCallExpression(Delegate call) : QxExpression
    {
        public List<QxExpression> Arguments { get; init; } = [];

        public Instance Invoke(params Instance[] arguments) => call.DynamicInvoke(arguments) as Instance ?? QxType.Void;
    }
}
