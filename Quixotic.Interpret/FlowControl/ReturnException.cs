using Quixotic.Interpret.Symbols.Values;

namespace Quixotic.Interpret.FlowControl
{
    public class ReturnException(Value? value) : FlowControlException
    {
        public Value? Value { get; } = value;
    }
}
