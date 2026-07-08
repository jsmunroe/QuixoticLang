using Quixotic.Common.Symbols;
using Quixotic.Common.TypeSystem.Types;

namespace Quixotic.Common.TypeSystem.Symbols
{
    public class TypeSymbol(string name, QxType type) : Symbol
    {
        public string Name { get; } = name;
        public QxType Type { get; } = type;
    }
}
