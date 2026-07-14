using Quixotic.Common.TypeSystem;

namespace Quixotic.Common.Symbols.Functions
{
    public class BoundConstructor(Instance boundInstance, Constructor constructor) : BoundFunction(boundInstance, constructor)
    {
        public BaseConstructor? Base { get; } = constructor.Base;
    }
}
