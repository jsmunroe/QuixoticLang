using Quixotic.Common.Contracts;
using Quixotic.Common.TypeSystem.Types;

namespace Quixotic.Common.Types
{
    public static class InstanceExtensions
    {
        public static IEnumerable<QxType> GetTypes<THasType>(this IEnumerable<THasType> source)
            where THasType : IHasType
        {
            return source.Select(i => i.Type);
        }
    }
}
