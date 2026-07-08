using Quixotic.Common.TypeSystem;
using Quixotic.Common.TypeSystem.Types;

namespace Quixotic.Runtime.References
{
    public class ArrayReference : CollectionReference
    {
        public ArrayReference(Instance instance) : base(instance)
        {
            if (instance.Type is not ArrayType)
                throw new InvalidOperationException($"Instance is not of type {QxType.Array(QxType.Any)}.");
        }

        public ArrayReference(QxType elementType, Instance[] elements) : base(QxType.Array(elementType))
        {
            QxType.Array(elementType).Assign(this, elements);
        }

        public void Set(int index, Instance value)
        {
            ((ArrayType)Type).Set(index, this, value);
        }

        public void Set(Instance index, Instance value)
        {
            ((ArrayType)Type).Set(index, this, value);
        }

        public Instance Get(int index)
        {
            return ((ArrayType)Type).Get(this, index);
        }

        public Instance Get(Instance index)
        {
            return ((ArrayType)Type).Get(this, index);
        }

        public static ArrayReference Convert(Instance instance)
        {
            if (instance is ArrayReference array)
                return array;

            return new ArrayReference(instance);
        }
    }
}
