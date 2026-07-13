using Quixotic.Common.Symbols;
using Quixotic.Common.Types;

namespace Quixotic.Common.TypeSystem.Symbols
{
    public class FunctionSymbol : SignatureSymbol
    {
        public FunctionSymbol(string name, Function function)
            : base(name, function.ReturnType, [.. function.Parameters.GetTypes()])
        {
            Function = function;
        }

        public FunctionSymbol(FunctionSymbol other)
            : base(other)
        {
            Function = other.Function;
        }

        public Function Function { get; }
    }
}
