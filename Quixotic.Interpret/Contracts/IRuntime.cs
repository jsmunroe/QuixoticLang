using Quixotic.Interpret.Environment;
using Quixotic.Interpret.Symbols.Instances;

namespace Quixotic.Interpret.Contracts
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