using Quixotic.Common.Symbols;
using Quixotic.Common.TypeSystem.Types;

namespace Quixotic.Common.TypeSystem.Symbols
{
    public class TypeSymbol(TypeName name, QxType type) : Symbol($"{nameof(TypeSymbol)}:{name}")
    {
        public TypeSymbol(TypeSymbol other)
            : this(other.Name, other.Type)
        { }

        public TypeName Name { get; } = name;

        public QxType Type { get; } = type;
    }
}
