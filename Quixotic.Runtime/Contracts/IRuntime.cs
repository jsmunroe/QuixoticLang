using Quixotic.Common.TypeSystem;
using Quixotic.Runtime.Environment;

namespace Quixotic.Runtime.Contracts
{
    public interface IRuntime
    {
        IRuntimeFrame Frame { get; }
        void ExecutePrint(Instance value);
        IRuntimeFrame Pop();
        IRuntimeFrame PushBlock(RuntimeFrameType type);
        IRuntimeFrame PushFunction();
    }
}