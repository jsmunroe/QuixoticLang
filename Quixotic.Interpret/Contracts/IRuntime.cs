using Quixotic.Interpret.Environment;
using Quixotic.Interpret.Symbols;

namespace Quixotic.Interpret.Contracts
{
    public interface IRuntime
    {
        IRuntimeFrame Frame { get; }

        void ExecutePrint(Value value);
        IRuntimeFrame Pop();
        IRuntimeFrame PushBlock(RuntimeFrameType type);
        IRuntimeFrame PushFunction();
    }
}