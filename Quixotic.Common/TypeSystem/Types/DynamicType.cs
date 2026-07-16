using Quixotic.Common.Exceptions.Interpret;
using Quixotic.Common.Symbols;
using Quixotic.Common.Symbols.Functions;
using Quixotic.Common.TypeSystem.Symbols;

namespace Quixotic.Common.TypeSystem.Types
{
    public class DynamicType() : QxType("dynamic")
    {
        public static DynamicType Default { get; } = new DynamicType();

        public override bool HasGenerics => false;

        public override bool Match(QxType actual, GenericBindings bindings)
        {
            return actual is DynamicType;
        }

        public Function BuildPropertyGetter(Instance target, string name)
        {
            var getter = PropertyGetter(name);
            var instance = getter.Invoke([target]);

            return BindableFunction.FromDelegate(this, getter, instance.Type, FunctionCallType.Getter);
        }

        public Function BuildPropertySetter(Instance target, string name, QxType type)
        {
            return BindableFunction.FromDelegate(this, PropertySetter(name), type, FunctionCallType.Setter, new Parameter("value", type));
        }

        public Function BuildPropertyCaller(Instance target, string name, QxType[] argumentTypes)
        {
            if (target[name] is not Instance value || !value.Type.Equals(Function))
                throw new UndefinedMethodException(target.Type, name);

            var function = Function.GetFunction(value);

            var bindableFunction = BindableFunction.Coerce(target.Type, function);
            return bindableFunction;
        }
    }
}
