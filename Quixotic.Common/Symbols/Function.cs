using Quixotic.Common.Statements;
using Quixotic.Common.TypeSystem.Types;

namespace Quixotic.Common.TypeSystem.Symbols
{
    public class Function(Block body, QxType returnType)
    {
        public List<Parameter> Parameters { get; init; } = [];

        public Block Body { get; } = body;

        public QxType ReturnType { get; internal set; } = returnType;

        public Function AddThis(Instance @this) => AddThis(@this.Type);

        public Function AddThis(QxType thisType)
        {
            return new(Body, ReturnType)
            {
                Parameters = [new Parameter("this", thisType), .. Parameters]
            };
        }
    }
}
