using Quixotic.Common.Symbols;
using Quixotic.Common.Symbols.Functions;
using Quixotic.Common.TypeSystem.Symbols;

namespace Quixotic.Common.TypeSystem.Types
{
    public class DynamicType() : QxType("dynamic")
    {
        public override bool HasGenerics => false;

        public override bool Match(QxType actual, GenericBindings bindings)
        {
            return actual is DynamicType;
        }

        public Function BuildPropertyGetter(Instance target, string name)
        {
            var getter = PropertyGetter(name);
            var instance = getter.Invoke(target);

            return Function.FromDelegate(getter, instance.Type, FunctionCallType.Getter, [new Parameter("this", target.Type)]);
        }

        public Function BuildPropertySetter(Instance target, string name, QxType type)
        {
            return Function.FromDelegate(PropertySetter(name), type, FunctionCallType.Getter, [new Parameter("this", target.Type), new Parameter("value", type)]);
        }
    }
}
