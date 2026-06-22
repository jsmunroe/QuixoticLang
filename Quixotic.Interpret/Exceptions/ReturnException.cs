using Quixotic.Interpret.Symbols;

namespace Quixotic.Interpret.Exceptions
{
    public class ReturnException(Value? value) : FlowControlException
    {
        public Value? Value { get; } = value;
    }
}
