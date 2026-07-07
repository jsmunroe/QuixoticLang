using Quixotic.Common.TypeSystem;
using Quixotic.Common.TypeSystem.Types;

namespace Quixotic.Interpret.FlowControl
{
    public class ReturnException(Instance? value) : FlowControlException
    {
        public Instance Value { get; } = value ?? QxType.Void;
    }
}
