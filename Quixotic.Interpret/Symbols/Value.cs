using Quixotic.Interpret.Exceptions;

namespace Quixotic.Interpret.Symbols
{

    public abstract record Value
    {
        private readonly object? _value;

        protected Value(ValueType type, object? value)
        {
            Type = type;
            _value = value;
        }

        public ValueType Type { get; }

        public virtual bool IsTruthy()
        {
            return Type.IsTruthy(this);
        }

        public object? Unwrap() => _value;

        public virtual Value Add(Value right)
        {
            throw new BinaryOperatorException(Type, "+", right.Type);
        }

        public virtual Value Subtract(Value right)
        {
            throw new BinaryOperatorException(Type, "-", right.Type);
        }

        public virtual Value Multiply(Value right)
        {
            throw new BinaryOperatorException(Type, "*", right.Type);
        }

        public virtual Value Divide(Value right)
        {
            throw new BinaryOperatorException(Type, "/", right.Type);
        }

        public virtual BooleanValue IsEqualTo(Value right)
        {
            if (Type != right.Type)
                return BooleanValue.False;

            return new BooleanValue(Equals(Unwrap(), right.Unwrap()));
        }

        public virtual BooleanValue IsNotEqualTo(Value right)
        {
            if (Type != right.Type)
                return BooleanValue.True;

            return new BooleanValue(!Equals(Unwrap(), right.Unwrap()));
        }

        public virtual BooleanValue IsLessThan(Value right)
        {
            throw new BinaryOperatorException(Type, "<", right.Type);
        }

        public virtual BooleanValue IsLessThanOrEqualTo(Value right)
        {
            throw new BinaryOperatorException(Type, "<", right.Type);
        }

        public virtual BooleanValue IsGreaterThan(Value right)
        {
            throw new BinaryOperatorException(Type, ">", right.Type);
        }

        public virtual BooleanValue IsGreaterThanOrEqualTo(Value right)
        {
            throw new BinaryOperatorException(Type, ">", right.Type);
        }

        public virtual BooleanValue And(Value right)
        {
            return new BooleanValue(IsTruthy() && right.IsTruthy());
        }

        public virtual BooleanValue Or(Value right)
        {
            return new BooleanValue(IsTruthy() || right.IsTruthy());
        }

    }

    public record NumberValue(double value) : Value(ValueType.Number, value)
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

    public record StringValue(string value) : Value(ValueType.String, value)
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

    public record BooleanValue(bool value) : Value(ValueType.Boolean, value)
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

    public record NadaValue() : Value(ValueType.Nada, null)
    {
        public override string ToString() => "nada";

        public override Value Add(Value right)
        {
            if (right is StringValue stringValue)
                return new StringValue("nada" + stringValue.Value);

            throw new BinaryOperatorException(Type, "+", right.Type);
        }
    }


}
