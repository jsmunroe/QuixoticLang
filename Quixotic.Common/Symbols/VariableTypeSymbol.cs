using Quixotic.Common.TypeSystem.Types;

namespace Quixotic.Common.Symbols
{
    public class VariableTypeSymbol(string name, QxType type) : Symbol(name)
    {
        public VariableTypeSymbol(VariableTypeSymbol variableTypeSymbol)
            : this(variableTypeSymbol.Name, variableTypeSymbol.Type)
        { }

        public QxType Type { get; } = type;
    }
}
