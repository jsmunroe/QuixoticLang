using Quixotic.Common.Exceptions.Interpret;
using Quixotic.Common.TypeSystem.Types;
using Quixotic.Runtime.Instances;
using Quixotic.Runtime.Values;

namespace Quixotic.Runtime.References
{

    public class ArrayReference(QxType elementType, IEnumerable<Instance> elements) : Reference(new QxArrayType(elementType))
    {
        public ArrayReference(QxType ElementType)
            : this(ElementType, [])
        { }

        public Instance[] Elements { get; } = [.. elements];

        public QxType ElementType { get; } = elementType;

        public override ArrayReference Add(Instance element)
        {
            if (element is ArrayReference array)
                return Concat(array);

            return Append(element);
        }

        public ArrayReference Concat(ArrayReference other)
        {
            if (!ElementType.IsAssignableFrom(other.ElementType))
                throw new TypeMismatchException(other.ElementType, ElementType);

            return new(ElementType, [.. Elements, .. other.Elements]);
        }

        public ArrayReference Append(Instance element)
        {
            if (!ElementType.IsAssignableFrom(element.Type))
                throw new TypeMismatchException(element.Type, ElementType);

            return new(ElementType, [.. Elements, element]);
        }

        public Instance Get(NumberValue index)
        {
            if (index.Value < 0 || index.Value >= Elements.Length)
                throw new OutOfRangeException(nameof(index));

            return Elements[(int)index.Value];
        }

        public void Set(NumberValue index, Instance element)
        {
            if (!ElementType.IsAssignableFrom(element.Type))
                throw new TypeMismatchException(ElementType, element.Type);

            if (index.Value < 0 || index.Value >= Elements.Length)
                throw new OutOfRangeException(nameof(index));

            Elements[(int)index.Value] = element;
        }

        public override bool IsTruthy()
        {
            return Elements.Length > 0;
        }

        public override string ToString()
        {
            return $"{{Qx:Array<{ElementType}>[{Elements.Length}]}}";
        }
    }
}
