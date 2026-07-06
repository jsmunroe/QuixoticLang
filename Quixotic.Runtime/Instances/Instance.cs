using Quixotic.Common.Contracts;
using Quixotic.Common.Exceptions.Interpret;
using Quixotic.Common.Types;
using Quixotic.Runtime.Values;

namespace Quixotic.Runtime.Instances
{
    public abstract class Instance(QxType type) : IHasType
    {
        public QxType Type { get; } = type;

        public bool IsNada => this is NadaInstance;

        public static Instance Nada { get; } = NadaInstance.Instance;

        public static Instance Void { get; } = VoidInstance.Instance;

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

        public static QxType GetCommonBase(IEnumerable<Instance> instances)
        {
            return QxType.GetCommonBase(instances.Select(i => i.Type));
        }
    }
}
