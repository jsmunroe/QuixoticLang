using Quixotic.Analysis.Environment;
using Quixotic.Common.Environment;

namespace Quixotic.Common.Contracts
{
    public static class FunctionProviderExtensions
    {
        public static void Register(this IFunctionProvider functionProvider, SignatureRegistry signatureRegistry)
        {
            var functionRegistry = new FunctionRegistry();

            functionProvider.Register(functionRegistry);

            foreach (var function in functionRegistry.AllFunctions)
                signatureRegistry.Register(function.Name, function.ReturnType, [.. function.ParameterTypes], function.CallType);
        }
    }
}
