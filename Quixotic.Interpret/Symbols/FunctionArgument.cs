using Quixotic.Interpret.Symbols.Instances;

namespace Quixotic.Interpret.Symbols
{
    public class Argument(string name, Instance instance)
    {
        public string Name { get; } = name;

        public Instance Value { get; } = instance;
    }

}
