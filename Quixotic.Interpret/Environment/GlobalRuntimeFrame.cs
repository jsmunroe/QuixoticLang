using Quixotic.Interpret.Contracts;

namespace Quixotic.Interpret.Environment
{
    public class GlobalRuntimeFrame : IRuntimeFrame
    {
        public IRuntimeFrame? Parent => null;

        public RuntimeFrameType Type => RuntimeFrameType.Global;

        public Scope Scope { get; } = new(null);

        public Scope GlobalScope => Scope;
    }
}
