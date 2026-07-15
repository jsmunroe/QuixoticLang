using Quixotic.Common.Environment;
using Quixotic.Common.Exceptions.Interpret;
using Quixotic.Common.Symbols.Functions;
using Quixotic.Runtime.Contracts;

namespace Quixotic.Runtime.Environment
{
    public class FunctionRuntimeFrame(IRuntimeFrame parent, Function function) : IRuntimeFrame
    {
        public IRuntimeFrame Parent { get; } = parent;

        public Function Function { get; } = function;

        public Scope Scope { get; } = new Scope(function.Closure ?? parent.GlobalScope);

        public Scope GlobalScope => Parent?.GlobalScope ?? throw new RuntimeException($"{nameof(FunctionRuntimeFrame)} cannot resolve global scope.");
    }
}
