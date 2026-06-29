using Quixotic.Common.Exceptions.Interpret;
using Quixotic.Interpret.Symbols.Values;
using System.Diagnostics.CodeAnalysis;

namespace Quixotic.Interpret.Symbols.Types
{
    public class QxType(string name)
    {
        public string Name { get; } = name;

        public bool Is(Value value) => IsAssignableFrom(value.Type);

        public override bool Equals(object? obj)
        {
            if (obj is not QxType other)
                return false;

            return string.Equals(Name, other.Name);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Name);
        }

        public virtual bool IsTruthy(Value value)
        {
            return false;
        }

        public virtual bool IsAssignableFrom(QxType subtype)
        {
            if (subtype.GetType() != GetType())
                return false;

            return true;
        }

        public static QxType Parse(string typeName)
        {
            return typeName.ToLower() switch
            {
                "number" => Number,
                "string" => String,
                "boolean" => Boolean,
                _ => throw new UnrecognizedTypeException(typeName)
            };
        }

        public static bool TryParse(string? typeName, [NotNullWhen(returnValue: true)] out QxType? type)
        {
            type = typeName?.ToLower() switch
            {
                "number" => Number,
                "string" => String,
                "boolean" => Boolean,
                _ => null
            };
            return type is not null;
        }

        public static QxType Number { get; } = NumberType.Instance;
        public static QxType String { get; } = StringType.Instance;
        public static QxType Boolean { get; } = BooleanType.Instance;
        public static QxType Nada { get; } = NadaType.Instance;
        public static QxType Void { get; } = VoidType.Instance;
    }
}
