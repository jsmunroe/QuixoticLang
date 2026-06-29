using Quixotic.Common.Exceptions.Interpret;
using Quixotic.Interpret.Symbols.Types;

namespace Quixotic.Interpret.Symbols.Values
{

    public abstract record Value
    {
        private readonly object? _value;

        protected Value(QxType type, object? value)
        {
            Type = type;
            _value = value;
        }

        public QxType Type { get; }

        public bool IsNada => this is NadaValue;


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

        public static Value Nada { get; } = NadaValue.Value;

    }
}
