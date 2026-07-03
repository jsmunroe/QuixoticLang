using Quixotic.Analysis.BuiltIn;
using Quixotic.Analysis.Contracts;

namespace Quixotic.Analysis.Environment
{
    public class GlobalFrame : IFrame
    {
        public GlobalFrame()
        {
            Symbols.Add(new BuiltInSignatures());
        }

        public IFrame? Parent => null;

        public SymbolTable Symbols { get; } = new();

        public FrameRedirectionType RedirectionType { get; set; } = FrameRedirectionType.None;

    }
}
