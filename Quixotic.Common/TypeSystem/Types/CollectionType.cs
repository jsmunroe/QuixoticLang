namespace Quixotic.Common.TypeSystem.Types
{
    public abstract class CollectionType(string name, QxType elementType, CollectionDefinition definition) : ConstructedGenericType(name, definition)
    {
        public QxType ElementType { get; } = elementType;

        public override bool HasGenerics => ElementType is Generic;

        public override bool IsTruthy(Instance instance)
        {
            if (instance["elements"] is Instance[] collection)
                return collection.Length > 0;

            return false;
        }

        public override bool IsAssignableFrom(QxType other)
        {
            if (other is not CollectionType collectionType)
                return false;

            if (!ElementType.IsAssignableFrom(collectionType.ElementType))
                return false;

            return true;
        }

        protected virtual Instance[] CleanElements(Instance[] elements)
        {
            return elements;
        }
    }
}
