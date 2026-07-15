using Quixotic.Common.Symbols.Functions;
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
        IRuntimeFrame PushFunction(Function function);
        IRuntimeFrame PushType(QxType type);
    }
}