using Quixotic.Common.TypeSystem;
using Quixotic.Common.TypeSystem.Types;

namespace Quixotic.Runtime.References
{
    public class SetReference : CollectionReference
    {
        public SetReference(Instance instance)
            : base(instance)
        {
            if (instance.Type is not SetType)
                throw new InvalidOperationException($"Instance is not of type {QxType.Set.MakeGenericType(QxType.Any)}.");
        }

        public SetReference(QxType elementType, Instance[] elements) : base(QxType.Set.MakeGenericType(elementType))
        {
            var type = (SetType)QxType.Set.MakeGenericType(elementType);
            type.Assign(this, elements);
        }


        public static SetReference Convert(Instance instance)
        {
            if (instance is SetReference set)
                return set;

            return new SetReference(instance);
        }
    }
}
