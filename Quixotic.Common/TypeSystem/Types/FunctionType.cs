using Quixotic.Common.Symbols.Functions;
using Quixotic.Common.Types;
using Quixotic.Common.TypeSystem.Symbols;

namespace Quixotic.Common.TypeSystem.Types
{
    public class FunctionType(QxType returnType, params Parameter[] parameters) : QxType(GetFunctionTypeName(returnType, parameters))
    {
        public Instance Construct(Function function)
        {
            var instance = new Instance(this);
            instance["function"] = function;

            return instance;
        }

        public override bool HasGenerics => false;

        public QxType ReturnType { get; } = returnType;
        public Parameter[] Parameters { get; } = parameters;

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

        protected static string GetFunctionTypeName(QxType returnType, params Parameter[] parameters)
        {
            return $"function({string.Join(", ", parameters.GetTypes())}): {returnType}";
        }
    }
}
