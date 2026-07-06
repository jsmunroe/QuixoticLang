using Quixotic.Common.TypeSystem.Types;

namespace Quixotic.Common.Symbols
{
    public class VariableTypeSymbol(string name, QxType type) : Symbol
    {
        public string Name { get; } = name;

        public QxType Type { get; } = type;
    }
}
