using Quixotic.Common.TypeSystem.Types;
using Quixotic.Runtime.Contracts;

namespace Quixotic.Runtime.Environment
{
    public class TypeRuntimeFrame(IRuntimeFrame parent, QxType type) : BlockRuntimeFrame(parent)
    {
        public QxType Type { get; } = type;
    }
}
