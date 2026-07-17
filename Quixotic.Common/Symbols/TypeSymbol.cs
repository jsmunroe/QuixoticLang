using Quixotic.Common.Symbols;
using Quixotic.Common.TypeSystem.Types;
using System.Diagnostics;

namespace Quixotic.Common.TypeSystem.Symbols
{
    [DebuggerDisplay("Qx:{Type.Name}")]
    public class TypeSymbol(TypeName name, QxType type) : Symbol($"{nameof(TypeSymbol)}:{name}")
    {
        public TypeSymbol(TypeSymbol other)
            : this(other.Name, other.Type)
        { }

        public QxType Type { get; } = type;
    }
}
