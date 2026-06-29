using Quixotic.Interpret.Symbols.Types;

namespace Quixotic.Interpret.Symbols
{
    public class Parameter(string name, QxType type)
    {
        public string Name { get; } = name;
        public QxType Type { get; } = type;
    }

}
