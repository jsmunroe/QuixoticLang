using Quixotic.Interpret.Symbols.Instances;

namespace Quixotic.Interpret.FlowControl
{
    public class ReturnException(Instance? value) : FlowControlException
    {
        public Instance Value { get; } = value ?? Instance.Void;
    }
}
