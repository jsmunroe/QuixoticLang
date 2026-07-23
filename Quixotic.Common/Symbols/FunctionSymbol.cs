using Quixotic.Common.Symbols;
using Quixotic.Common.Symbols.Functions;
using Quixotic.Common.Types;

namespace Quixotic.Common.TypeSystem.Symbols
{
    public class FunctionSymbol : SignatureSymbol
    {
        public FunctionSymbol(string name, Function function)
            : base(name, function.ReturnType, [.. function.Parameters.GetTypes()], function.CallType)
        {
            Function = function;
        }

        public FunctionSymbol(FunctionSymbol other)
            : base(other)
        {
            Function = other.Function;
        }

        public override FunctionSymbol Substitute(GenericBindings bindings)
        {
            var function = Function.Substitute(bindings);

            return new(Name, function);
        }

        public Function Function { get; }
    }
}
