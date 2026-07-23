using Quixotic.Analysis.Contracts;
using Quixotic.Common.TypeSystem.BuiltIn;

namespace Quixotic.Analysis.Environment
{
    public class GlobalFrame : IFrame
    {
        public GlobalFrame()
        {
            Symbols.Add(new BuiltInFunctions());
            Symbols.Add(new BuiltInTypes());
        }

        public IFrame? Parent => null;

        public SymbolTable Symbols { get; } = new();

        public FrameRedirectionType RedirectionType { get; set; } = FrameRedirectionType.None;

    }
}
