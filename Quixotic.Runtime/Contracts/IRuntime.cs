using Quixotic.Common.TypeSystem;

namespace Quixotic.Runtime.Contracts
{
    public interface IRuntime
    {
        IRuntimeFrame Frame { get; }
        void ExecutePrint(Instance value);
        IRuntimeFrame Pop();
        IRuntimeFrame PushBlock();
        IRuntimeFrame PushFunction();
    }
}