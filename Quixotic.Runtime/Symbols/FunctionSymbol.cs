using Quixotic.Common.Symbols;
using Quixotic.Common.Types;

namespace Quixotic.Runtime.Symbols
{
    public class FunctionSymbol(string name, Function function) : FunctionSignatureSymbol(name, function.ReturnType, [.. GetParameterTypes(function)])
    {
        public Function Function { get; } = function;

        private static List<QxType> GetParameterTypes(Function function) => [.. function.Parameters.Select(p => p.Type)];
    }
}
