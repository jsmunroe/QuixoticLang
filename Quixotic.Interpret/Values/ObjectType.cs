namespace Quixotic.Interpret.Values
{
    public class ValueType
    {
        private readonly Type _typeOfValue;

        protected ValueType(string name, Type typeOfValue)
        {
            _typeOfValue = typeOfValue;

            Name = name;
        }

        public string Name { get; }

        public bool Is(object value) => _typeOfValue.IsInstanceOfType(value);


        public override bool Equals(object? obj)
        {
            if (!base.Equals(obj) || obj is not ValueType other)
                return false;

            return other._typeOfValue.Equals(_typeOfValue);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Name, _typeOfValue);
        }

        public virtual bool IsTruthy(Value value)
        {
            return false;
        }

        public static ValueType Number { get; } = NumberType.Instance;
        public static ValueType String { get; } = StringType.Instance;
        public static ValueType Boolean { get; } = BooleanType.Instance;
        public static ValueType Nada { get; } = NadaType.Instance;
    }

    public class NumberType : ValueType
    {
        public static NumberType Instance { get; } = new();

        protected NumberType()
            : base("number", typeof(NumberValue))
        { }

        public override bool IsTruthy(Value value)
        {
            if (value is not NumberValue numberValue)
                return false;

            return numberValue.Value != 0;
        }
    }

    public class StringType : ValueType
    {
        public static StringType Instance { get; } = new();

        protected StringType()
            : base("string", typeof(StringValue))
        { }

        public override bool IsTruthy(Value value)
        {
            if (value is not StringValue numberValue)
                return false;

            return numberValue.Value != string.Empty;
        }
    }

    public class BooleanType : ValueType
    {
        public static BooleanType Instance { get; } = new();

        protected BooleanType()
            : base("boolean", typeof(BooleanType))
        { }

        public override bool IsTruthy(Value value)
        {
            if (value is not BooleanValue booleanValue)
                return false;

            return booleanValue.Value;
        }
    }

    public class NadaType : ValueType
    {
        public static NadaType Instance { get; } = new();

        protected NadaType()
            : base("nada", typeof(NadaValue))
        { }

        public override bool IsTruthy(Value value)
        {
            return false;
        }
    }
}
