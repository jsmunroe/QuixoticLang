using Quixotic.Common.TypeSystem;

namespace Quixotic.Runtime.Symbols
{
    public class Argument(string name, Instance instance)
    {
        public string Name { get; } = name;

        public Instance Value { get; } = instance;
    }

}
