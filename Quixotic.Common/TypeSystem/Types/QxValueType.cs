namespace Quixotic.Common.TypeSystem.Types
{
    public abstract class QxValueType : QxType
    {
        private readonly Type _typeOfValue;

        protected QxValueType(string name, Type typeOfValue)
            : base(name)
        {
            _typeOfValue = typeOfValue;
        }

        public override bool HasGenerics => false;

        public override string ToString(Instance instance)
        {
            return instance["value"]?.ToString() ?? "nada";
        }

        public override bool Equals(Instance first, Instance second)
        {
            return Equals(first["value"], second["value"]);
        }

        public override int GetHashCode(Instance instance)
        {
            return instance["value"]?.GetHashCode() ?? 0;
        }

        public override bool Equals(object? obj)
        {
            if (obj is not QxValueType other)
                return false;

            if (!base.Equals(other))
                return false;

            return other._typeOfValue.Equals(_typeOfValue);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Name);
        }
    }
}
