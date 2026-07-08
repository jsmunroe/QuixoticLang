using Quixotic.Runtime.BuiltIn;
using Quixotic.Runtime.Contracts;

namespace Quixotic.Runtime.Environment
{
    public class GlobalRuntimeFrame : IRuntimeFrame
    {
        public GlobalRuntimeFrame()
        {
            Scope.Add(new BuiltInFunctions());
        }

        public IRuntimeFrame? Parent => null;

        public Scope Scope { get; } = new Scope(null);

        public Scope GlobalScope => Scope;
    }
}
