namespace Quixotic.Interpret.Symbols
{
    public class Argument(string name, Value value)
    {
        public string Name { get; } = name;

        public Value Value { get; } = value;
    }

}
