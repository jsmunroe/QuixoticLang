using Quixotic.Analysis.Environment;

namespace Quixotic.Analysis.Contracts
{
    public interface IFrame
    {
        IFrame? Parent { get; }

        SymbolTable Symbols { get; }

        FrameRedirectionType RedirectionType { get; set; }
    }

    public enum FrameRedirectionType
    {
        None,
        Continue,
        Break,
        Return,
    }

    public static class FrameExtensions
    {
        public static SymbolTable GetGlobalSymbols(this IFrame frame)
        {
            var currentFrame = frame;
            while (currentFrame.Parent is not null)
                currentFrame = currentFrame.Parent;

            return currentFrame.Symbols;
        }

        public static FunctionFrame? GetFunctionFrame(this IFrame frame)
        {
            var currentFrame = frame;

            while (currentFrame is not null)
            {
                if (currentFrame is FunctionFrame functionFrame)
                    return functionFrame;

                currentFrame = currentFrame.Parent;
            }

            return null;
        }

        public static LoopFrame? GetLoopFrame(this IFrame frame)
        {
            var currentFrame = frame;

            while (currentFrame is not null)
            {
                if (currentFrame is FunctionFrame)
                    return null;

                if (currentFrame is LoopFrame loopFrame)
                    return loopFrame;

                currentFrame = currentFrame.Parent;
            }

            return null;
        }
    }
}