using Quixotic.Common.Exceptions.Interpret;

namespace Quixotic.Interpret.Symbols
{

    public record NumberValue(double value) : Value(QxType.Number, value)
    {
        public double Value { get; } = value;

        public override string ToString() => Value.ToString();

        public override Value Add(Value right)
        {
            if (right is NumberValue numberValue)
                return new NumberValue(Value + numberValue.Value);

            if (right is StringValue stringValue)
                return new StringValue(Value.ToString() + stringValue.Value);

            throw new BinaryOperatorException(Type, "+", right.Type);
        }

        public override Value Subtract(Value right)
        {
            if (right is NumberValue numberValue)
                return new NumberValue(Value - numberValue.Value);

            throw new BinaryOperatorException(Type, "-", right.Type);
        }

        public override Value Multiply(Value right)
        {
            if (right is NumberValue numberValue)
                return new NumberValue(Value * numberValue.Value);

            throw new BinaryOperatorException(Type, "*", right.Type);
        }

        public override Value Divide(Value right)
        {
            if (right is NumberValue numberValue)
                return new NumberValue(Value / numberValue.Value);

            throw new BinaryOperatorException(Type, "/", right.Type);
        }

        public override BooleanValue IsLessThan(Value right)
        {
            if (right is NumberValue numberValue)
                return new BooleanValue(Value < numberValue.Value);

            throw new BinaryOperatorException(Type, "<", right.Type);
        }
        public override BooleanValue IsLessThanOrEqualTo(Value right)
        {
            if (right is NumberValue numberValue)
                return new BooleanValue(Value <= numberValue.Value);

            throw new BinaryOperatorException(Type, "<=", right.Type);
        }


        public override BooleanValue IsGreaterThan(Value right)
        {
            if (right is NumberValue numberValue)
                return new BooleanValue(Value > numberValue.Value);

            throw new BinaryOperatorException(Type, ">", right.Type);
        }

        public override BooleanValue IsGreaterThanOrEqualTo(Value right)
        {
            if (right is NumberValue numberValue)
                return new BooleanValue(Value >= numberValue.Value);

            throw new BinaryOperatorException(Type, ">=", right.Type);
        }

    }
}