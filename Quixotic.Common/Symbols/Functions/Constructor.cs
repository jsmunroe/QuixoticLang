using Quixotic.Common.Statements;
using Quixotic.Common.TypeSystem.Symbols;
using Quixotic.Common.TypeSystem.Types;

namespace Quixotic.Common.Symbols.Functions
{
    public class Constructor(Block body) : Function(body, QxType.Void.Type, FunctionCallType.Call)
    {
        public Constructor(Constructor other)
            : this(other.Body)
        {
            Base = other.Base;
        }

        public BaseConstructor? Base { get; init; }

        public override Function AddThis(QxType thisType)
        {
            return new Constructor(Body)
            {
                Parameters = [new Parameter("this", thisType), .. Parameters],
                Base = Base,
            };
        }
    }
}
