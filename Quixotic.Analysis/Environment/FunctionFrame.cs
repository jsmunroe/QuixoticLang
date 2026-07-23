using Quixotic.Analysis.Contracts;
using Quixotic.Common.TypeSystem.Symbols;

namespace Quixotic.Analysis.Environment
{
    public class FunctionFrame(IFrame parent, FunctionSymbol function, SymbolTable? symbolTable) : IFrame
    {
        public IFrame? Parent => parent;

        public SymbolTable Symbols { get; } = new(symbolTable ?? parent.GetGlobalSymbols());

        public FunctionSymbol Function { get; } = function;

        public FrameRedirectionType RedirectionType { get; set; } = FrameRedirectionType.None;
    }
}
