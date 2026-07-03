using Quixotic.Common.Exceptions.Interpret;
using Quixotic.Interpret.Symbols.Instances;
using Quixotic.Common.Types;

namespace Quixotic.Interpret.Symbols.Values
{

    public class ArrayInstance(QxType elementType, IEnumerable<Instance> elements) : Reference(new QxArrayType(elementType))
    {
        public ArrayInstance(QxType ElementType)
            : this(ElementType, [])
        { }

        public Instance[] Elements { get; } = [.. elements];

        public QxType ElementType { get; } = elementType;

        public override ArrayInstance Add(Instance element)
        {
            if (element is ArrayInstance array)
                return Concat(array);

            return Append(element);
        }

        public ArrayInstance Concat(ArrayInstance other)
        {
            if (!ElementType.IsAssignableFrom(other.ElementType))
                throw new TypeMismatchException(other.ElementType, ElementType);

            return new(ElementType, [.. Elements, .. other.Elements]);
        }

        public ArrayInstance Append(Instance element)
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
