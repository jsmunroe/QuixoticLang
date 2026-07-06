using Quixotic.Common.Exceptions.Interpret;
using Quixotic.Common.TypeSystem.Types;
using Quixotic.Runtime.Instances;

namespace Quixotic.Runtime.Values
{

    public class NumberValue(double value) : Value(QxType.Number)
    {
        public double Value { get; } = value;

        public override Instance Add(Instance right)
        {
            if (right is NumberValue numberValue)
                return new NumberValue(Value + numberValue.Value);

            throw new BinaryOperatorException(Type, "+", right.Type);
        }

        public override Instance Subtract(Instance right)
        {
            if (right is NumberValue numberValue)
                return new NumberValue(Value - numberValue.Value);

            throw new BinaryOperatorException(Type, "-", right.Type);
        }

        public override Instance Multiply(Instance right)
        {
            if (right is NumberValue numberValue)
                return new NumberValue(Value * numberValue.Value);

            throw new BinaryOperatorException(Type, "*", right.Type);
        }

        public override Instance Divide(Instance right)
        {
            if (right is NumberValue numberValue)
                return new NumberValue(Value / numberValue.Value);

            throw new BinaryOperatorException(Type, "/", right.Type);
        }

        public override BooleanValue IsLessThan(Instance right)
        {
            if (right is NumberValue numberValue)
                return new BooleanValue(Value < numberValue.Value);

            throw new BinaryOperatorException(Type, "<", right.Type);
        }
        public override BooleanValue IsLessThanOrEqualTo(Instance right)
        {
            if (right is NumberValue numberValue)
                return new BooleanValue(Value <= numberValue.Value);

            throw new BinaryOperatorException(Type, "<=", right.Type);
        }


        public override BooleanValue IsGreaterThan(Instance right)
        {
            if (right is NumberValue numberValue)
                return new BooleanValue(Value > numberValue.Value);

            throw new BinaryOperatorException(Type, ">", right.Type);
        }

        public override BooleanValue IsGreaterThanOrEqualTo(Instance right)
        {
            if (right is NumberValue numberValue)
                return new BooleanValue(Value >= numberValue.Value);

            throw new BinaryOperatorException(Type, ">=", right.Type);
        }

        public override object? Unwrap()
        {
            return Value;
        }

        public override bool Equals(Instance other)
        {
            return other is NumberValue numberValue && Value == numberValue.Value;
        }

        public override bool IsTruthy()
        {
            return Value != 0;
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public override string ToString()
        {
            return Value.ToString();
        }
    }
}