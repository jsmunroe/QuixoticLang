using Quixotic.Analysis.Contracts;
using Quixotic.Common.TypeSystem.Types;

namespace Quixotic.Analysis.Environment
{
    public class TypeFrame(IFrame parent, QxType type) : IFrame
    {
        public IFrame? Parent => parent;

        public SymbolTable Symbols { get; } = new(parent.GetGlobalSymbols());

        public QxType Type { get; } = type;

        public FrameRedirectionType RedirectionType { get; set; } = FrameRedirectionType.None;
    }
}
