using Quixotic.Interpret.Exceptions;

namespace Quixotic.Interpret.Symbols
{
    public class VariableSymbol(Value value) : Symbol
    {
        public Value Value { get; private set; } = value;

        public ValueType Type { get; } = value.Type;

        public void Assign(Value value)
        {
            if (Type != Symbols.ValueType.Nada && !value.Type.Equals(Type))
                throw new TypeMismatchException(Type, value.Type);

            Value = value;
        }
    }

    public class FunctionSymbol(Function function) : Symbol
    {
        public Function Function { get; } = function;
    }
}
