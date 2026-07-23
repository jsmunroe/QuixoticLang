using Quixotic.Common.TypeSystem;
using Quixotic.Common.TypeSystem.Symbols;

namespace Quixotic.Common.Symbols.Functions
{
    public class BoundConstructor(Instance boundInstance, Constructor constructor) : BoundFunction(boundInstance, constructor)
    {
        public BaseConstructor? Base { get; } = constructor.Base;

        public override BoundConstructor Substitute(GenericBindings bindings)
        {
            var function = base.Substitute(bindings);

            var constructor = new Constructor(BoundInstance.Type, Body);
            return new BoundConstructor(BoundInstance, constructor);
        }

        public List<Argument> BindArguments(Instance[] instances)
        {
            var name = $"{BoundInstance.Type}::constructor";

            return base.BindArguments(name, instances);
        }
    }
}
