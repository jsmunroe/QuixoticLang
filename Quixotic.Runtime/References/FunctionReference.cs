using Quixotic.Common.Symbols.Functions;
using Quixotic.Common.TypeSystem;
using Quixotic.Common.TypeSystem.Types;

namespace Quixotic.Runtime.References
{
    public class FunctionReference : Instance
    {
        public FunctionReference() : base(QxType.Function)
        { }

        public FunctionReference(Instance instance) : base(instance)
        { }

        public Function Function
        {
            get => QxType.Function.GetFunction(this);
            set => QxType.Function.SetFunction(this, value);
        }
    }
}
