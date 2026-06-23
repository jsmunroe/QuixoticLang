namespace Quixotic.Interpret.Symbols
{
    public class QxType
    {
        private readonly Type _typeOfValue;

        protected QxType(string name, Type typeOfValue)
        {
            _typeOfValue = typeOfValue;

            Name = name;
        }

        public string Name { get; }

        public bool Is(object value) => _typeOfValue.IsInstanceOfType(value);


        public override bool Equals(object? obj)
        {
            if (obj is not QxType other)
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

        public static QxType GetType<TValue>()
            where TValue : Value
        {
            return typeof(TValue) switch
            {
                var t when t == typeof(NumberValue) => Number,
                var t when t == typeof(StringType) => String,
                var t when t == typeof(BooleanType) => Boolean,
                var t when t == typeof(NadaType) => Nada,
                _ => throw new NotImplementedException($"Type {typeof(TValue).Name} is not implemented.")
            };
        }

        public static QxType Number { get; } = NumberType.Instance;
        public static QxType String { get; } = StringType.Instance;
        public static QxType Boolean { get; } = BooleanType.Instance;
        public static QxType Nada { get; } = NadaType.Instance;
    }

    public class NumberType : QxType
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

    public class StringType : QxType
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

    public class BooleanType : QxType
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

    public class NadaType : QxType
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
