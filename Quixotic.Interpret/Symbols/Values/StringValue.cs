using Quixotic.Common.Exceptions.Interpret;
using Quixotic.Interpret.Symbols.Instances;
using Quixotic.Common.Types;
namespace Quixotic.Interpret.Symbols.Values
{
    public class StringValue(string value) : Value(QxType.String)
    {
        public string Value { get; } = value;

        public override Instance Add(Instance right)
        {
            if (right is StringValue stringValue)
                return new StringValue(Value + stringValue.Value);

            if (right is NumberValue numberValue)
                return new StringValue(Value + numberValue.ToString());

            if (right is BooleanValue booleanValue)
                return new StringValue(Value + booleanValue.ToString());

            if (right is NadaValue)
                return new StringValue(Value + "nada");

            throw new BinaryOperatorException(Type, "+", right.Type);
        }

        public override object? Unwrap()
        {
            return Value;
        }

        public override bool Equals(Instance other)
        {
            return other is StringValue stringValue && Value == stringValue.Value;
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public override bool IsTruthy()
        {
            return !string.IsNullOrEmpty(Value);
        }

        public override string ToString()
        {
            return Value;
        }
    }
}