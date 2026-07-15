using Quixotic.Common.Environment;

namespace Quixotic.Runtime.Contracts
{
    public interface IRuntimeFrame
    {
        IRuntimeFrame? Parent { get; }
        Scope Scope { get; }
        Scope GlobalScope { get; }
    }
}