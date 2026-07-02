using Quixotic.Common.Symbols;

namespace Quixotic.Interpret.Symbols
{
    public class FunctionSymbol(Function function) : FunctionTypeSymbol(function.ReturnType)
    {
        public Function Function { get; } = function;
    }
}
