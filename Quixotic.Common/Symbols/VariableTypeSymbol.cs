using Quixotic.Common.TypeSystem.Types;

namespace Quixotic.Common.Symbols
{
    public class VariableTypeSymbol(string name, QxType type) : Symbol($"{nameof(VariableTypeSymbol)}:{name}")
    {
        public VariableTypeSymbol(VariableTypeSymbol variableTypeSymbol)
            : this(variableTypeSymbol.Name, variableTypeSymbol.Type)
        { }

        public string Name { get; } = name;

        public QxType Type { get; } = type;
    }
}
