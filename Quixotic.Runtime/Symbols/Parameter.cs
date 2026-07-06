using Quixotic.Common.Contracts;
using Quixotic.Common.Types;

namespace Quixotic.Runtime.Symbols
{
    public class Parameter(string name, QxType type) : IHasType
    {
        public string Name { get; } = name;
        public QxType Type { get; } = type;
    }

}
