using Quixotic.Common.Exceptions.Interpret;
using Quixotic.Runtime.Contracts;

namespace Quixotic.Runtime.Environment
{
    public class FunctionRuntimeFrame(IRuntimeFrame parent) : IRuntimeFrame
    {
        public IRuntimeFrame? Parent { get; } = parent;

        public RuntimeFrameType Type => RuntimeFrameType.Function;

        public Scope Scope { get; } = new(parent?.GlobalScope);

        public Scope GlobalScope => Parent?.GlobalScope ?? throw new RuntimeException($"{nameof(FunctionRuntimeFrame)} cannot resolve global scope.");
    }
}
