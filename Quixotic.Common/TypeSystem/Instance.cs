using Quixotic.Common.Contracts;
using Quixotic.Common.TypeSystem.Types;

namespace Quixotic.Common.TypeSystem
{
    public class Instance(QxType type) : IHasType
    {
        public Instance(Instance other) : this(other.Type)
        {
            Fields = new Dictionary<string, object?>(other.Fields);
        }

        protected Instance(QxType type, Instance instance)
            : this(type)
        {
            Fields = instance.Fields;
        }

        public QxType Type { get; } = type;

        protected Dictionary<string, object?> Fields { get; } = [];

        public object? this[string name]
        {
            get => Fields.TryGetValue(name, out var value) ? value : null;
            set => Fields[name] = value;
        }

        public override string ToString()
        {
            return Type.ToString(this);
        }

        public bool IsTruthy => Type.IsTruthy(this);

        public bool IsNada => QxType.IsNada(this);

        public override bool Equals(object? obj)
        {
            if (obj is not Instance other)
                return false;

            return Type.Equals(this, other);
        }

        public override int GetHashCode()
        {
            return Type.GetHashCode(this);
        }
    }
}
