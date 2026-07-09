using Quixotic.Common.TypeSystem;
using Quixotic.Common.TypeSystem.Types;

namespace Quixotic.Runtime.Contracts
{
    public interface IRuntime
    {
        IRuntimeFrame Frame { get; }
        void ExecutePrint(Instance value);
        IRuntimeFrame Pop();
        IRuntimeFrame PushBlock();
        IRuntimeFrame PushFunction();
        IRuntimeFrame PushType(QxType type);
    }
}