using Quixotic.Common.Symbols;
using Quixotic.Common.Types;

namespace Quixotic.Common.TypeSystem.Symbols
{
    public class FunctionSymbol(string name, Function function) : SignatureSymbol(name, function.ReturnType, [.. function.Parameters.GetTypes()])
    {
        public Function Function { get; } = function;
    }
}
