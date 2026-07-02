using Quixotic.Common.Statements;
using Quixotic.Common.Types;

namespace Quixotic.Interpret.Symbols
{
    public class Function(Block body, QxType returnType)
    {
        public List<Parameter> Parameters { get; init; } = [];

        public Block Body { get; } = body;

        public QxType ReturnType { get; internal set; } = returnType;
    }


}
