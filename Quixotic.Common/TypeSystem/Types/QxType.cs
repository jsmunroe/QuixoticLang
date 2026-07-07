using Quixotic.Common.Contracts;
using Quixotic.Common.Environment;
using Quixotic.Common.Exceptions.Interpret;
using Quixotic.Common.TypeSystem.Symbols;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace Quixotic.Common.TypeSystem.Types
{
    public class QxType(string name)
    {
        private static readonly Regex _rexTypeString = new(@"^([a-zA-Z_][a-zA-Z0-9_\.]+)(\[\])?$", RegexOptions.Compiled);

        private FunctionRegistry _methods = new();

        public string Name { get; } = name;

        public override int GetHashCode()
        {
            return HashCode.Combine(Name);
        }

        public virtual bool IsAssignableFrom(QxType subtype)
        {
            if (this == Any || this is Generic)
                return true;

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

        public static QxType GetCommonBase(IEnumerable<Instance> instances)
        {
            return GetCommonBase(instances.Select(i => i.Type));
        }

        public bool Is(Instance instance)
        {
            return Equals(instance.Type);
        }

        public virtual string ToString(Instance instance)
        {
            return $"{{Qx:{Name}}}";
        }

        public Function GetMemberMethod(Instance thisInstance, string name, params Instance[] arguments)
        {
            if (!_methods.TryResolve(name, [.. arguments.Select(a => a.Type)], out var functionSymbol))
                throw new UndefinedMethodException(this, name);

            return functionSymbol.Function;
        }

        public void AddMethods(IFunctionProvider methodSource)
        {
            methodSource.Register(_methods);
        }

        public virtual bool Equals(Instance first, Instance second)
        {
            return false;
        }

        public virtual int GetHashCode(Instance instance)
        {
            return 0;
        }

        public virtual bool IsTruthy(Instance instance)
        {
            return false;
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

        public void RegisterMethod(string name, Delegate method, QxType returnValue, params Parameter[] parameters)
        {
            _methods.Register(name, method, returnValue, parameters);
        }

        public static bool IsNada(Instance instance)
        {
            return (instance == Nada);
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
                "nada" => Nada.Type,
                "void" => Void.Type,
                _ => null
            };

            if (type is null)
                return false;

            if (isArray)
                type = new ArrayType(type);

            return true;
        }

        public static QxType Any { get; } = new("any");

        public static NumberType Number { get; } = NumberType.Instance;
        public static StringType String { get; } = StringType.Instance;
        public static BooleanType Boolean { get; } = BooleanType.Instance;
        public static Instance Nada { get; } = NadaType.Value;
        public static Instance Void { get; } = VoidType.Value;

        public static ArrayType Array(QxType elementType) => new ArrayType(elementType);

        public static Generic Generic(string name) => new Generic(name);
    }
}
