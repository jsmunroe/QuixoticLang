using Quixotic.Common.Exceptions.Interpret;
using Quixotic.Interpret.Symbols.Types;
namespace Quixotic.Interpret.Symbols.Values
{
    public record StringValue(string value) : Value(QxType.String, value)
    {
        public string Value { get; } = value;

        public override string ToString() => Value;

        public override Value Add(Value right)
        {
            if (right is StringValue stringValue)
                return new StringValue(Value + stringValue.Value);

            if (right is NumberValue numberValue)
                return new StringValue(Value + numberValue.ToString());

            if (right is BooleanValue booleanValue)
                return new StringValue(Value + booleanValue.ToString());

            if (right is NadaValue nadaValue)
                return new StringValue(Value + "nada");

            throw new BinaryOperatorException(Type, "+", right.Type);
        }
    }
}