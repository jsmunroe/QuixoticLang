using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace Quixotic.Common.Types
{
    public class QxType(string name)
    {
        private static readonly Regex _rexTypeString = new(@"^([a-zA-Z_][a-zA-Z0-9_\.]+)(\[\])?$", RegexOptions.Compiled);

        public string Name { get; } = name;

        public override int GetHashCode()
        {
            return HashCode.Combine(Name);
        }

        public virtual bool IsAssignableFrom(QxType subtype)
        {
            if (ReferenceEquals(this, Any))
                return true;

            if (Equals(subtype))
                return true;

            return false;
        }

        public virtual QxType GetCommonBase(QxType other)
        {
            if (this.IsAssignableFrom(other))
                return this;

            if (other.IsAssignableFrom(this))
                return other;

            return Any;
        }

        public static QxType GetCommonBase(IEnumerable<QxType> types)
        {
            QxType? commonBase = null;

            foreach (var type in types)
            {
                if (commonBase is null)
                    commonBase = type;
                else
                    commonBase = commonBase.GetCommonBase(type);
            }

            return commonBase ?? Any;
        }

        public override string ToString()
        {
            return Name;
        }

        public override bool Equals(object? obj)
        {
            if (obj is not QxType other)
                return false;

            return string.Equals(Name, other.Name);
        }

        public static QxType Parse(string typeName)
        {
            return TryParse(typeName, out var type) ? type : throw new InvalidOperationException($"Unrecognized type: {typeName}");
        }

        public static bool TryParse(string? typeName, [NotNullWhen(returnValue: true)] out QxType? type)
        {
            type = null;

            if (typeName is null)
                return false;

            var match = _rexTypeString.Match(typeName);

            if (!match.Success)
                return false;

            typeName = match.Groups[1].Value;
            var isArray = match.Groups[2].Success;

            type = typeName?.ToLower() switch
            {
                "number" => Number,
                "string" => String,
                "boolean" => Boolean,
                "nada" => Nada,
                "void" => Void,
                _ => null
            };

            if (type is null)
                return false;

            if (isArray)
                type = new ArrayType(type);

            return true;
        }

        public static QxType Any { get; } = new("any");

        public static QxType Number { get; } = NumberType.Instance;
        public static QxType String { get; } = StringType.Instance;
        public static QxType Boolean { get; } = BooleanType.Instance;
        public static QxType Nada { get; } = NadaType.Instance;
        public static QxType Void { get; } = VoidType.Instance;

        public static QxType Array(QxType elementType) => new ArrayType(elementType);
    }
}
