using Quixotic.Common.Types;

namespace Quixotic.Runtime.Symbols.Values
{

    public class BooleanValue(bool value) : Value(QxType.Boolean)
    {
        public bool Value { get; } = value;

        public BooleanValue Not()
        {
            return new BooleanValue(!Value);
        }

        public override bool Equals(Instance other)
        {
            if (other is BooleanValue booleanValue && booleanValue.Value == Value)
                return true;

            return false;
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public override bool IsTruthy()
        {
            return Value;
        }

        public override object? Unwrap()
        {
            return Value;
        }

        public static BooleanValue True { get; } = new(true);

        public static BooleanValue False { get; } = new(false);

        public override string ToString()
        {
            return Value ? "true" : "false";
        }
    }

}