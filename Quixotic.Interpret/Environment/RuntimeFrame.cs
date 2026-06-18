using Quixotic.Interpret.Values;

namespace Quixotic.Interpret.Environment
{
    public class RuntimeFrame(RuntimeFrameType type, RuntimeFrame? parent = null)
    {
        public RuntimeFrame? Parent { get; init; } = parent;

        public RuntimeFrameType Type { get; } = type;

        public Scope LocalScope { get; } = new();

        public bool ContainsName(string name)
        {
            return LocalScope.ContainsName(name) || (Parent?.ContainsName(name) ?? false);
        }

        public Value this[string name]
        {
            get { return LocalScope[name] ?? Parent?[name] ?? new NadaValue(); }
            set
            {
                if (Parent?.ContainsName(name) == true)
                    Parent[name] = value;

                LocalScope[name] = value;
            }
        }
    }
}
