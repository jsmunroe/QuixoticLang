using Quixotic.Common.Syntax.Casing;
using Quixotic.Common.TypeSystem;
using Quixotic.Common.TypeSystem.Types;

namespace Quixotic.Interop.TypeSystem.Types
{
    public class ClrType(Type externalType, QxType? baseType = null) : DefinedType(CaseRule.Current.TypeNames.Recase(externalType.Name), baseType)
    {
        public Type ExternalType { get; } = externalType;

        public override bool HasGenerics => false;

        public Instance Construct(object clrObject)
        {
            if (clrObject.GetType() != ExternalType)
                throw new Exception($"This ClrType can only construct types of {ExternalType.Name}.");

            var instance = new Instance(this);
            instance["instance"] = clrObject;

            return instance;
        }

        public override bool Match(QxType actual, GenericBindings bindings)
        {
            return actual is ClrType other &&
                   ExternalType == other.ExternalType;
        }

        public override string ToString(Instance instance)
        {
            var clrInstance = instance["instance"];

            return clrInstance?.ToString() ?? base.ToString(instance);
        }
    }
}
