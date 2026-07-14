using Quixotic.Common.TypeSystem.BuiltIn;
using Quixotic.Runtime.BuiltIn;
using Quixotic.Runtime.Contracts;

namespace Quixotic.Runtime.Environment
{
    public class GlobalRuntimeFrame : IRuntimeFrame
    {
        public GlobalRuntimeFrame()
        {
            Scope.Add(new BuiltInFunctions());
            Scope.Add(new BuiltInTypes());

            GlobalScope = Scope;
        }

        public IRuntimeFrame? Parent => null;

        public Scope Scope { get; } = new Scope(null);

        public Scope GlobalScope { get; }
    }
}
