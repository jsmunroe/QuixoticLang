using Quixotic.Common.Exceptions.Interpret;

namespace Quixotic.Interpret.Symbols
{
    public class VariableSymbol : Symbol
    {
        public Value? Value { get; private set; }

        public QxType Type { get; }

        public VariableSymbol(Value value)
        {
            Value = value;
            Type = value.Type;
        }

        public VariableSymbol(QxType valueType)
        {
            Value = NadaValue.Value;
            Type = valueType;
        }

        public void Assign(Value value)
        {
            if (Type != QxType.Nada && !value.Type.Equals(Type))
                throw new TypeMismatchException(Type, value.Type);

            Value = value;
        }
    }

    public class FunctionSymbol(Function function) : Symbol
    {
        public Function Function { get; } = function;
    }
}
