namespace Quixotic.Common.TypeSystem.Types
{
    public class QxValueType : QxType
    {
        private readonly Type _typeOfValue;

        protected QxValueType(string name, Type typeOfValue)
            : base(name)
        {
            _typeOfValue = typeOfValue;
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
