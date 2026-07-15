using Quixotic.Common.Symbols.Functions;

namespace Quixotic.Common.TypeSystem.Types
{
    public class FunctionType() : QxType("function")
    {
        public static FunctionType Default { get; } = new();

        public Instance Construct(Function function)
        {
            var instance = new Instance(this);
            instance["function"] = function;

            return instance;
        }

        public override bool HasGenerics => false;

        public override bool Match(QxType actual, GenericBindings bindings)
        {
            return actual is FunctionType;
        }

        public Function GetFunction(Instance instance)
        {
            return (Function)instance["function"]!;
        }

        public void SetFunction(Instance instance, Function function)
        {
            instance["function"] = function;
        }
    }
}
