using Quixotic.Common.Statements;

namespace Quixotic.Interpret.Symbols
{
    public class Function(Block body)
    {
        public List<Parameter> Parameters { get; init; } = [];

        public Block Body { get; } = body;
    }


}
