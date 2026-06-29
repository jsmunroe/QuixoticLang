using Quixotic.Common.Exceptions.Interpret;
using Quixotic.Interpret.Symbols.Types;

namespace Quixotic.Interpret.Symbols.Values
{

    public record BooleanValue(bool value) : Value(QxType.Boolean, value)
    {
        public bool Value { get; } = value;

        public override string ToString() => Value == true ? "true" : "false";

        public override Value Add(Value right)
        {
            if (right is StringValue stringValue)
                return new StringValue(Value + stringValue.Value);

            throw new BinaryOperatorException(Type, "+", right.Type);
        }

        public BooleanValue Not()
        {
            return new BooleanValue(!Value);
        }

        public static BooleanValue True { get; } = new(true);

        public static BooleanValue False { get; } = new(false);
    }

}