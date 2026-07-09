using Quixotic.Common.Contracts;
using Quixotic.Common.Environment;
using Quixotic.Common.Exceptions.Interpret;
using Quixotic.Common.Types;
using Quixotic.Common.TypeSystem.Symbols;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace Quixotic.Common.TypeSystem.Types
{
    public class QxType(string name)
    {
        private static readonly Regex _rexTypeString = new(@"^([a-zA-Z_][a-zA-Z0-9_\.]+)(\[\])?$", RegexOptions.Compiled);

        private readonly FunctionRegistry _methods = new();

        private readonly Dictionary<string, object?> _initialState = new();

        public string Name { get; } = name;

        public virtual Instance Construct()
        {
            var instance = new Instance(this);

            foreach (var (name, value) in _initialState)
                instance[name] = value;

            return instance;
        }

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

        public void Add(IFunctionProvider functionProvider)
        {
            functionProvider.Register(_methods);
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
            parameters = [new Parameter("this", this), .. parameters];
            _methods.Register(name, method, returnValue, parameters);
        }

        public void RegisterMethod(string name, Function function)
        {
            _methods.Register(name, function.AddThis(this));
        }

        public void RegisterProperty(string name, Instance initialValue)
        {
            var type = initialValue.Type;
            _initialState[name] = initialValue;
            RegisterProperty(name, type);
        }

        public void RegisterProperty(string name, QxType type)
        {
            RegisterMethod(name, PropertyGetter(name), type);
            RegisterMethod(name, PropertySetter(name), type, new Parameter("value", type));
        }

        private Func<Instance, Instance> PropertyGetter(string name) => (Instance instance) => (Instance)instance[name]!;

        private Action<Instance, Instance> PropertySetter(string name) => (Instance instance, Instance value) => instance[name] = value;

        public Function ResolveMethod(Instance thisInstance, string name, params Instance[] arguments)
        {
            Instance[] allArguments = [thisInstance, .. arguments];

            if (!_methods.TryResolve(name, [.. allArguments.GetTypes()], out var functionSymbol))
                throw new UndefinedMethodException(this, name);

            return functionSymbol.Function;
        }

        public bool TryResolveMethod(Instance thisInstance, string name, Instance[] arguments, [NotNullWhen(returnValue: true)] out Function? function)
        {
            function = null;

            Instance[] allArguments = [thisInstance, .. arguments];

            if (!_methods.TryResolve(name, [.. allArguments.GetTypes()], out var functionSymbol))
                return false;

            function = functionSymbol.Function;
            return true;
        }

        /// <summary>
        /// For static methods
        /// </summary>
        public Function ResolveMethod(string name, params Instance[] arguments)
        {
            if (!_methods.TryResolve(name, [.. arguments.GetTypes()], out var functionSymbol))
                throw new UndefinedMethodException(this, name);

            return functionSymbol.Function;
        }

        /// <summary>
        /// For static methods
        /// </summary>
        public bool TryResolveMethod(string name, Instance[] arguments, [NotNullWhen(returnValue: true)] out Function? function)
        {
            function = null;

            if (!_methods.TryResolve(name, [.. arguments.GetTypes()], out var functionSymbol))
                return false;

            function = functionSymbol.Function;
            return true;
        }



        public bool IsMemberDeclared(string name)
        {
            return _methods.Contains(name);
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

        public static ArrayType Array(QxType elementType) => new(elementType);
        public static SetType Set(QxType elementType) => new(elementType);
        public static CollectionType Collection(QxType elementType) => new($"collection<{elementType}>", elementType);
        public static CollectionType Collection(CollectionType collectionType, QxType elementType)
        {
            return collectionType switch
            {
                ArrayType => new ArrayType(elementType),
                SetType => new SetType(elementType),
                _ => throw new InvalidOperationException($"Unsupported collection type: {collectionType.Name}")
            };
        }

        public static Generic Generic(string name) => new(name);
    }
}
