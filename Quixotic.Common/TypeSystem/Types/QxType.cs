using Quixotic.Common.Contracts;
using Quixotic.Common.Environment;
using Quixotic.Common.Exceptions.Interpret;
using Quixotic.Common.Symbols;
using Quixotic.Common.Symbols.Functions;
using Quixotic.Common.Syntax.Casing;
using Quixotic.Common.Tokens;
using Quixotic.Common.Types;
using Quixotic.Common.TypeSystem.Symbols;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Quixotic.Common.TypeSystem.Types
{
    [DebuggerDisplay("{Name,nq}")]
    public abstract class QxType
    {
        private readonly MethodRegistry _methods;

        private readonly Dictionary<string, object?> _initialState = new(CaseRule.Current.StringComparer);

        public QxType(string name, QxType? baseType = null)
        {
            Name = name;
            BaseType = baseType;

            _methods = new(this);
        }

        public TypeName Name { get; }

        public QxType? BaseType { get; }

        public virtual Instance Construct()
        {
            var instance = new Instance(this);

            foreach (var (name, value) in _initialState)
                instance[name] = value;

            return instance;
        }

        public Instance Upcast(Instance instance)
        {
            if (!IsAssignableFrom(instance.Type))
                throw new CastMismatchException(instance.Type, this, Span.Empty); // TODO: Figure out actual Span

            return new TypedInstanceView(instance, this);
        }

        public bool Is(Instance instance)
        {
            return IsAssignableFrom(instance.Type);
        }

        public abstract bool Match(QxType actual, GenericBindings bindings);

        public override int GetHashCode()
        {
            return HashCode.Combine(Name);
        }

        public virtual bool IsAssignableFrom(QxType other)
        {
            if (other == Any)
                return true;

            for (QxType? current = other; current is not null; current = current.BaseType)
            {
                if (Equals(current))
                    return true;
            }

            return false;
        }

        public bool IsAssignableFrom(Instance instance) => IsAssignableFrom(instance.Type);

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

            return string.Equals(Name, other.Name, CaseRule.Current.StringComparison);
        }

        public void RegisterConstructor(Constructor constructor)
        {
            RegisterMethod("::constructor", constructor);
        }

        public void RegisterConstructor(ExternalFunction method, params Parameter[] parameters)
        {
            _methods.RegisterConstructor(method, parameters);
        }

        public virtual void RegisterMethod(string name, ExternalFunction method, QxType returnType, params Parameter[] parameters)
        {
            _methods.RegisterBindable(name, method, returnType, FunctionCallType.Call, parameters);
        }

        public virtual void RegisterMethod(string name, Function function)
        {
            _methods.RegisterBindable(name, function);
        }

        public virtual void RegisterStaticMethod(string name, ExternalFunction method, QxType returnType, params Parameter[] parameters)
        {
            _methods.Register(name, method, returnType, FunctionCallType.Call, parameters);
        }

        public virtual void RegisterStaticMethod(string name, Function function)
        {
            _methods.Register(name, function);
        }

        public virtual void RegisterProperty(string name, Instance initialValue)
        {
            var type = initialValue.Type;
            RegisterProperty(name, type, initialValue);
        }

        public virtual void RegisterProperty(string name, QxType type, Instance? initialValue = null, bool isReadOnly = false)
        {
            _initialState[name] = initialValue ?? Nada;

            _methods.RegisterBindable(name, PropertyGetter(name), type, FunctionCallType.Getter);

            if (!isReadOnly)
                _methods.RegisterBindable(name, PropertySetter(name), type, FunctionCallType.Call, new Parameter("value", type));
        }

        public virtual void RegisterProperty(string name, QxType type, ExternalFunction getter, ExternalFunction? setter = null, Instance? initialValue = null)
        {
            _initialState[name] = initialValue ?? Nada;

            _methods.RegisterBindable(name, getter, type, FunctionCallType.Getter);

            if (setter is not null)
                _methods.RegisterBindable(name, setter, type, FunctionCallType.Call, new Parameter("value", type));
        }

        public virtual void RegisterStaticProperty(string name, QxType type, Instance? initialValue = null, bool isReadOnly = false)
        {
            _initialState[name] = initialValue ?? Nada;

            _methods.Register(name, PropertyGetter(name), type, FunctionCallType.Getter);

            if (!isReadOnly)
                _methods.Register(name, PropertySetter(name), type, FunctionCallType.Call, new Parameter("value", type));
        }

        public virtual void RegisterStaticProperty(string name, QxType type, ExternalFunction getter, ExternalFunction? setter = null, Instance? initialValue = null)
        {
            _initialState[name] = initialValue ?? Nada;

            _methods.Register(name, getter, type, FunctionCallType.Getter);

            if (setter is not null)
                _methods.Register(name, setter, type, FunctionCallType.Call, new Parameter("value", type));
        }

        protected ExternalFunction PropertyGetter(string name) => (Instance[] args) =>
        {
            return (Instance)args[0][name]! ?? NadaType.Value;
        };

        protected ExternalFunction PropertySetter(string name) => (Instance[] args) =>
        {
            args[0][name] = args[1];
            return args[1];
        };

        public Function ResolveMethod(Instance thisInstance, string name, params Instance[] arguments)
        {
            if (!TryResolveMethod(thisInstance, name, arguments, out var function))
                throw new UndefinedMethodException(this, name, Span.Empty); // TODO: Figure out actual Span

            return function;
        }

        public bool TryResolveMethod(Instance thisInstance, string name, Instance[] arguments, [NotNullWhen(returnValue: true)] out Function? function)
        {
            function = null;

            if (_methods.TryResolve(name, [.. arguments.GetTypes()], out var functionSymbol))
            {
                function = functionSymbol.Function;
                return true;
            }

            return BaseType?.TryResolveMethod(thisInstance, name, arguments, out function) ?? false;

        }

        /// <summary>
        /// For static methods
        /// </summary>
        public Function ResolveMethod(string name, params Instance[] arguments)
        {
            if (!TryResolveMethod(name, arguments, out var function))
                throw new UndefinedMethodException(this, name, Span.Empty); // TODO: Figure out actual Span

            return function;
        }

        /// <summary>
        /// For static methods
        /// </summary>
        public bool TryResolveMethod(string name, Instance[] arguments, [NotNullWhen(returnValue: true)] out Function? function)
        {
            function = null;

            if (_methods.TryResolve(name, [.. arguments.GetTypes()], out var functionSymbol))
            {
                function = functionSymbol.Function;
                return true;
            }

            return BaseType?.TryResolveMethod(name, arguments, out function) ?? false;
        }

        public Constructor ResolveConstructor(params Instance[] arguments)
        {
            if (!TryResolveConstructor(arguments, out var constructor))
                throw new UndefinedConstructorException(this);

            return constructor;
        }

        public bool TryResolveConstructor(Instance[] arguments, [NotNullWhen(returnValue: true)] out Constructor? constructor)
        {
            constructor = null;

            if (_methods.TryResolve("::constructor", [.. arguments.GetTypes()], out var functionSymbol))
            {
                constructor = functionSymbol.Function as Constructor;
                return constructor is not null;
            }

            return false;
        }

        public IEnumerable<Function> ResolveMethods(string name)
        {
            return _methods.Resolve(name).Select(f => f.Function);
        }

        public bool IsMemberDeclared(string name)
        {
            return _methods.Contains(name);
        }

        public abstract bool HasGenerics { get; }

        public virtual QxType Substitute(GenericBindings bindings)
        {
            return this;
        }

        public static bool IsNada(Instance instance)
        {
            return (instance == Nada);
        }

        public static QxMetaType Meta(QxType typeReference) => new(typeReference);

        public static QxType Any { get; } = AnyType.Default;

        public static NumberType Number { get; } = NumberType.Default;
        public static StringType String { get; } = StringType.Default;
        public static BooleanType Boolean { get; } = BooleanType.Default;
        public static Instance Nada { get; } = NadaType.Value;
        public static Instance Void { get; } = VoidType.Value;

        public static FunctionType Function { get; } = FunctionType.Default;

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
