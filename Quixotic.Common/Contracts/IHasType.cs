using Quixotic.Common.TypeSystem.Types;

namespace Quixotic.Common.Contracts
{
    public interface IHasType
    {
        QxType Type { get; }
    }
}
