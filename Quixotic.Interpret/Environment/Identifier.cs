using Quixotic.Interpret.Exceptions;
using Quixotic.Interpret.Values;

namespace Quixotic.Interpret.Environment
{
    public class Identifier(Value value)
    {
        public Value Value { get; private set; } = value;

        public Values.ValueType Type { get; } = value.Type;

        public void Assign(Value value)
        {
            if (Type != Values.ValueType.Nada && !value.Type.Equals(Type))
                throw new TypeMismatchException(Type, value.Type);

            Value = value;
        }
    }
}
