using Quixotic.Common.Symbols;
using Quixotic.Common.Symbols.Functions;
using Quixotic.Common.Types;
using Quixotic.Common.TypeSystem.Symbols;
using Quixotic.Common.TypeSystem.Types;

namespace Quixotic.Common.Environment
{
    public class MethodRegistry(QxType instanceType) : FunctionRegistry
    {
        public QxType InstanceType { get; } = instanceType;

        public void RegisterBindable(string name, Delegate implementation, QxType returnType, FunctionCallType callType, params Parameter[] parameters)
        {
            var function = BindableFunction.FromDelegate(InstanceType, implementation, returnType, callType, parameters);

            var parameterTypes = function.Parameters.Select(p => p.Type);

            var signature = new Signature(name, [.. parameterTypes]);
            var functionSymbol = new FunctionSymbol(name, function);

            Register(signature, functionSymbol);
        }

        public void RegisterBindable(string name, Function function)
        {
            var parameterTypes = function.Parameters.GetTypes();

            var signature = new Signature(name, [.. parameterTypes]);

            function = function is BindableFunction bindableFunction ? bindableFunction : new BindableFunction(InstanceType, function);

            var functionSymbol = new FunctionSymbol(name, function);

            Register(signature, functionSymbol);
        }
    }
}
