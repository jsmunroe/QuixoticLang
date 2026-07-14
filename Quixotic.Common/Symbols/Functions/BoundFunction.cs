using Quixotic.Common.TypeSystem;
using Quixotic.Common.TypeSystem.Symbols;

namespace Quixotic.Common.Symbols.Functions
{
    public class BoundFunction : Function
    {
        public BoundFunction(Instance boundInstance, BindableFunction function) : base(function)
        {
            BoundInstance = boundInstance;

            Parameters.Insert(0, new Parameter("this", boundInstance.Type));
        }

        public Instance BoundInstance { get; }
    }
}
