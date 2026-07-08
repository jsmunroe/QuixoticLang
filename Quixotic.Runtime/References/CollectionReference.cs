using Quixotic.Common.TypeSystem;
using Quixotic.Common.TypeSystem.Types;

namespace Quixotic.Runtime.References
{
    public class CollectionReference : Instance
    {
        public CollectionReference(Instance instance) : base(instance)
        { }

        public CollectionReference(CollectionType type) : base(type)
        { }

        public QxType ElementType => ((CollectionType)Type).ElementType;

        public Instance[] Elements => (Instance[])this["elements"]!;

    }
}
