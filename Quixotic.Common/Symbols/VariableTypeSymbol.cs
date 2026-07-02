using Quixotic.Common.Types;

namespace Quixotic.Common.Symbols
{
    public class VariableTypeSymbol(QxType type) : Symbol
    {
        public QxType Type { get; } = type;
    }
}
