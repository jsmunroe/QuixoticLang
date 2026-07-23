using Quixotic.Common.Environment;
using Quixotic.Common.Exceptions.Interpret;
using Quixotic.Common.Symbols;
using Quixotic.Common.Symbols.Functions;
using Quixotic.Common.Syntax.Casing;
using Quixotic.Common.Tokens;
using Quixotic.Common.TypeSystem.BuiltIn;
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

        private bool _methodsLoaded = false;

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

        protected void EnforceMethodsLoaded()
        {
            if (_methodsLoaded)
                return;

            RegisterStaticMethod("and", (args) => BooleanType.Default.Construct(args[0].IsTruthy && args[1].IsTruthy), BooleanType.Default, new Parameter("left", Any), new Parameter("right", Any));
            RegisterStaticMethod("or", (args) => BooleanType.Default.Construct(args[0].IsTruthy || args[1].IsTruthy), BooleanType.Default, new Parameter("left", Any), new Parameter("right", Any));

            RegisterStaticMethod("in", (args) => args[1].Type is CollectionType collectionType ? CollectionDefinition.Contains(args[1], args[0]) : BooleanType.False, BooleanType.Default, new Parameter("this", this), new Parameter("collection", Array.MakeGenericType(Generic("TItem"))));
            RegisterStaticMethod("in", (args) => args[1].Type is CollectionType collectionType ? CollectionDefinition.Contains(args[1], args[0]) : BooleanType.False, BooleanType.Default, new Parameter("this", this), new Parameter("collection", Set.MakeGenericType(Generic("TItem"))));

            LoadMethods();

            _methodsLoaded = true;
        }

        protected virtual void LoadMethods()
        {
            // Optionally handled by subclasses of QxType
        }

        public FunctionSymbol RegisterConstructor(Constructor constructor)
        {
            return RegisterMethod("::constructor", constructor);
        }

        public FunctionSymbol RegisterConstructor(ExternalFunction method, params Parameter[] parameters)
        {
            return _methods.RegisterConstructor(method, parameters);
        }

        public virtual FunctionSymbol RegisterMethod(string name, ExternalFunction method, QxType returnType, params Parameter[] parameters)
        {
            return _methods.RegisterBindable(name, method, returnType, CallType.Call, parameters);
        }

        public virtual FunctionSymbol RegisterMethod(string name, Function function)
        {
            return _methods.RegisterBindable(name, function);
        }

        public virtual FunctionSymbol RegisterStaticMethod(string name, ExternalFunction method, QxType returnType, params Parameter[] parameters)
        {
            return _methods.Register(name, method, returnType, CallType.Call, parameters);
        }

        public virtual FunctionSymbol RegisterStaticMethod(string name, Function function)
        {
            return _methods.Register(name, function);
        }

        public virtual (FunctionSymbol, FunctionSymbol?) RegisterProperty(string name, Instance initialValue)
        {
            var type = initialValue.Type;
            return RegisterProperty(name, type, initialValue);
        }

        public virtual (FunctionSymbol, FunctionSymbol?) RegisterProperty(string name, QxType type, Instance? initialValue = null, bool isReadOnly = false)
        {
            _initialState[name] = initialValue ?? Nada;

            var getter = _methods.RegisterBindable(name, PropertyGetter(name), type, CallType.Getter);

            if (!isReadOnly)
            {
                var setter = _methods.RegisterBindable(name, PropertySetter(name), type, CallType.Call, new Parameter("value", type));
                return (getter, setter);
            }

            return (getter, null);
        }

        public virtual (FunctionSymbol, FunctionSymbol?) RegisterProperty(string name, QxType type, ExternalFunction getter, ExternalFunction? setter = null, Instance? initialValue = null)
        {
            _initialState[name] = initialValue ?? Nada;

            var getterSymbol = _methods.RegisterBindable(name, getter, type, CallType.Getter);

            if (setter is not null)
            {
                var setterSymbol = _methods.RegisterBindable(name, setter, type, CallType.Call, new Parameter("value", type));
                return (getterSymbol, setterSymbol);
            }

            return (getterSymbol, null);
        }

        public virtual (FunctionSymbol, FunctionSymbol?) RegisterStaticProperty(string name, QxType type, Instance? initialValue = null, bool isReadOnly = false)
        {
            _initialState[name] = initialValue ?? Nada;

            var getter = _methods.Register(name, PropertyGetter(name), type, CallType.Getter);

            if (!isReadOnly)
            {
                var setter = _methods.Register(name, PropertySetter(name), type, CallType.Call, new Parameter("value", type));
                return (getter, setter);
            }

            return (getter, null);
        }

        public virtual (FunctionSymbol, FunctionSymbol?) RegisterStaticProperty(string name, QxType type, ExternalFunction getter, ExternalFunction? setter = null, Instance? initialValue = null)
        {
            _initialState[name] = initialValue ?? Nada;

            var getterSymbol = _methods.Register(name, getter, type, CallType.Getter);

            if (setter is not null)
            {
                var setterSymbol = _methods.Register(name, setter, type, CallType.Call, new Parameter("value", type));
                return (getterSymbol, setterSymbol);
            }

            return (getterSymbol, null);
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

        public Function ResolveMethod(string name, params QxType[] arguments)
        {
            EnforceMethodsLoaded();

            if (!TryResolveMethod(name, arguments, out var function))
                throw new UndefinedMethodException(this, name, Span.Empty); // TODO: Figure out actual Span

            return function;
        }

        public virtual bool TryResolveMethod(string name, QxType[] arguments, [NotNullWhen(returnValue: true)] out Function? function)
        {
            EnforceMethodsLoaded();

            function = null;

            if (_methods.TryResolve(name, arguments, out var functionSymbol))
            {
                function = functionSymbol.Function;
                return true;
            }

            return BaseType?.TryResolveMethod(name, arguments, out function) ?? false;
        }

        public Constructor ResolveConstructor(params QxType[] arguments)
        {
            if (!TryResolveConstructor(arguments, out var constructor))
                throw new UndefinedConstructorException(this);

            return constructor;
        }

        public virtual bool TryResolveConstructor(QxType[] arguments, [NotNullWhen(returnValue: true)] out Constructor? constructor)
        {
            EnforceMethodsLoaded();

            constructor = null;

            if (_methods.TryResolve("::constructor", arguments, out var functionSymbol))
            {
                constructor = functionSymbol.Function as Constructor;
                return constructor is not null;
            }

            return false;
        }

        public bool IsMemberDeclared(string name)
        {
            EnforceMethodsLoaded();

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

        public static QxMetaType Meta(QxType typeReference) => BuiltInTypes.Meta(typeReference);

        public static QxType Any => BuiltInTypes.Any;

        public static NumberType Number => BuiltInTypes.Number;
        public static StringType String => BuiltInTypes.String;
        public static BooleanType Boolean => BuiltInTypes.Boolean;
        public static Instance Nada => BuiltInTypes.Nada;
        public static Instance Void => BuiltInTypes.Void;

        public static FunctionDefinition Function => BuiltInTypes.Function;

        public static ArrayDefinition Array => BuiltInTypes.Array;
        public static SetDefinition Set => BuiltInTypes.Set;
        public static CollectionType Collection(CollectionType collectionType, QxType elementType) => BuiltInTypes.Collection(collectionType, elementType);

        public static Generic Generic(string name) => BuiltInTypes.Generic(name);
    }
}
