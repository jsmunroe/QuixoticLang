using Quixotic.Analysis.Contracts;
using Quixotic.Common.Symbols;

namespace Quixotic.Analysis.Environment
{
    public class FunctionFrame(IFrame parent, SignatureSymbol function) : IFrame
    {
        public bool IsContinued { get; set; }

        public IFrame? Parent => parent;

        public SymbolTable Symbols { get; } = new(parent.GetGlobalSymbols());

        public SignatureSymbol Function { get; } = function;

        public FrameRedirectionType RedirectionType { get; set; } = FrameRedirectionType.None;
    }
}
