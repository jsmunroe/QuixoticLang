using Quixotic.Interpret.Environment;

namespace Quixotic.Interpret.Contracts
{
    public interface IRuntimeFrame
    {
        IRuntimeFrame? Parent { get; }
        RuntimeFrameType Type { get; }
        Scope Scope { get; }
        Scope GlobalScope { get; }
    }
}