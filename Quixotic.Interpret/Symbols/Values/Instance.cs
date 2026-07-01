using Quixotic.Common.Exceptions.Interpret;
using Quixotic.Interpret.Symbols.Types;
using Quixotic.Interpret.Symbols.Values;

namespace Quixotic.Interpret.Symbols.Instances
{
    public abstract class Instance(QxType type)
    {
        public QxType Type { get; } = type;

        public bool IsNada => this is NadaValue;

        public static Instance Nada { get; } = NadaValue.Value;

        public abstract bool Equals(Instance other);

        public abstract override int GetHashCode();

        public abstract bool IsTruthy();

        public override string ToString()
        {
            return $"{{ Qx:{Type} }}";
        }

        public virtual Instance Add(Instance right)
        {
            throw new BinaryOperatorException(Type, "+", right.Type);
        }

        public virtual Instance Subtract(Instance right)
        {
            throw new BinaryOperatorException(Type, "-", right.Type);
        }

        public virtual Instance Multiply(Instance right)
        {
            throw new BinaryOperatorException(Type, "*", right.Type);
        }

        public virtual Instance Divide(Instance right)
        {
            throw new BinaryOperatorException(Type, "/", right.Type);
        }

        public virtual BooleanValue IsEqualTo(Instance right)
        {
            return Equals(right) ? BooleanValue.True : BooleanValue.False;
        }

        public virtual BooleanValue IsNotEqualTo(Instance right)
        {
            return Equals(right) ? BooleanValue.False : BooleanValue.True;
        }

        public virtual BooleanValue IsLessThan(Instance right)
        {
            throw new BinaryOperatorException(Type, "<", right.Type);
        }

        public virtual BooleanValue IsLessThanOrEqualTo(Instance right)
        {
            throw new BinaryOperatorException(Type, "<", right.Type);
        }

        public virtual BooleanValue IsGreaterThan(Instance right)
        {
            throw new BinaryOperatorException(Type, ">", right.Type);
        }

        public virtual BooleanValue IsGreaterThanOrEqualTo(Instance right)
        {
            throw new BinaryOperatorException(Type, ">", right.Type);
        }

        public virtual BooleanValue And(Instance right)
        {
            return new BooleanValue(IsTruthy() && right.IsTruthy());
        }

        public virtual BooleanValue Or(Instance right)
        {
            return new BooleanValue(IsTruthy() || right.IsTruthy());
        }

    }
}
