using Quixotic.Analysis.Contracts;

namespace Quixotic.Analysis.Environment
{
    public class BlockFrame(IFrame parent) : IFrame
    {
        private FrameRedirectionType _redirectionType = FrameRedirectionType.None;

        public IFrame Parent => parent;

        public SymbolTable Symbols { get; } = new(parent.Symbols);

        public FrameRedirectionType RedirectionType
        {
            get => _redirectionType;
            set
            {
                _redirectionType = value;
                Parent.RedirectionType = value;
            }
        }
    }
}
