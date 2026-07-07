namespace Quixotic.Common.TypeSystem.Symbols
{
    public class Argument(string name, Instance instance)
    {
        public string Name { get; } = name;

        public Instance Value { get; } = instance;
    }

}
