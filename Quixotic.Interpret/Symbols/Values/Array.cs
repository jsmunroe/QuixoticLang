using Quixotic.Common.Exceptions.Interpret;
using Quixotic.Interpret.Symbols.Types;
using System.Collections.Immutable;

namespace Quixotic.Interpret.Symbols.Values
{
    public record Array(QxType ElementType, Reference[] Elements) : Reference(ArrayType.WithElement(ElementType))
    {
        private ImmutableArray<Reference> _elements = [];

        public Array(QxType ElementType)
            : this(ElementType, [])
        { }

        public Array Add(Reference element)
        {
            if (ElementType.IsAssignableFrom(element.Type))
                throw new TypeMismatchException(element.Type, ElementType);

            return this with
            {
                _elements = _elements.Add(element)
            };
        }

        public Array Add(Array elements)
        {
            if (!ElementType.IsAssignableFrom(elements.ElementType))
                throw new TypeMismatchException(elements.ElementType, ElementType);

            return this with
            {
                _elements = _elements.AddRange(elements._elements)
            };
        }
    }
}
