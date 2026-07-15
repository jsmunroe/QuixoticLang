using Quixotic.Common.Environment;
using Quixotic.Common.Statements;
using Quixotic.Common.TypeSystem.Types;

namespace Quixotic.Common.Symbols.Functions
{
    public class LambdaFunction : Function
    {
        public LambdaFunction(Block body, QxType returnType, FunctionCallType callType, Scope closure) : base(body, returnType, callType)
        {
            Closure = closure;
        }

        public LambdaFunction(Function other, Scope closure) : base(other)
        {
            Closure = closure;
        }

        public Scope Closure { get; }
    }
}
