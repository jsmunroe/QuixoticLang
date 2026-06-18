using Quixotic.Interpret.Environment;
using Quixotic.Interpret.Values;

namespace Quixotic.Interpret.Contracts
{
    public interface IRuntime
    {
        RuntimeFrame Frame { get; }

        void ExecutePrint(Value value);
        RuntimeFrame Pop();
        RuntimeFrame Push(RuntimeFrameType type);
    }
}