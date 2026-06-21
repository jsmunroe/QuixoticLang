using Quixotic.Interpret.Symbols;

namespace Quixotic.Interpret.Exceptions
{
    public class ReturnException(Value? value) : Exception
    {
        public Value? Value { get; } = value;
    }
}
