using Quixotic.Analysis.Contracts;

namespace Quixotic.Analysis.Environment
{
    public class LoopFrame(IFrame parent) : IFrame
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

                if (value == FrameRedirectionType.Return)
                    Parent.RedirectionType = value;
            }
        }

    }
}
