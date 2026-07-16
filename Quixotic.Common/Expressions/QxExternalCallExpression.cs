using Quixotic.Common.Environment;
using Quixotic.Common.TypeSystem;

namespace Quixotic.Common.Expressions
{
    public class QxExternalCallExpression(ExternalFunction call) : QxExpression
    {
        public List<QxExpression> Arguments { get; init; } = [];

        public Instance Invoke(params Instance[] arguments) => call(arguments);
    }
}
