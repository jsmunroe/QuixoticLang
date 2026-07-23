using Quixotic.Common.TypeSystem.Symbols;

namespace Quixotic.Common.TypeSystem.Types
{
    public abstract class CollectionDefinition(string name) : GenericTypeDefinition(name)
    {
        public override bool HasGenerics => true;

        public Generic ElementType = new("TItem");

        protected override void LoadMethods()
        {
            RegisterMethod("length", args => Number.Construct(Length(args[0])), Number);

            var definition = this;

            RegisterStaticMethod("+", args => Append(args[0], args[1]), definition, new Parameter("array", this), new Parameter("element", Generic("TItem")));
            RegisterStaticMethod("+", args => Append(args[0], args[1]), definition, new Parameter("array", this), new Parameter("other", definition));
        }

        public CollectionType MakeGenericType(QxType elementType)
        {
            var bindings = new GenericBindings();
            bindings.TryBind("TItem", elementType);
            return (CollectionType)MakeGenericType(bindings);
        }

        public override QxType Substitute(GenericBindings bindings)
        {
            return MakeGenericType(bindings);
        }

        public static int Length(Instance instance)
        {
            if (instance["elements"] is not Instance[] elements)
                throw new InvalidOperationException("Instance does not contain a collection of elements.");

            return elements.Length;
        }

        public Instance Append(Instance collection, Instance value)
        {
            if (collection.Type is not CollectionType collectionType)
                throw new InvalidOperationException("Right instance is not a collection type.");

            if (collection["elements"] is not Instance[] elements)
                throw new InvalidOperationException("Collection instance does not contain a collection of elements.");

            if (!collectionType.ElementType.IsAssignableFrom(value.Type))
                throw new InvalidOperationException($"Value is not of type {collectionType.ElementType}.");

            Instance[] newElements = [.. elements, value];

            return new Instance(Collection(collectionType, collectionType.ElementType))
            {
                ["elements"] = CleanElements(newElements)
            };
        }

        public Instance Concat(Instance collection, Instance other)
        {
            if (collection.Type is not CollectionType collectionType)
                throw new InvalidOperationException("Right instance is not a collection type.");

            if (other.Type is not CollectionType otherSetType)
                throw new InvalidOperationException("Left instance is not a collection type.");

            if (!collectionType.ElementType.IsAssignableFrom(otherSetType.ElementType))
                throw new InvalidOperationException($"Elements of type {otherSetType.ElementType} are not assignable to a collection of type {collectionType.ElementType}");

            if (collection["elements"] is not Instance[] elements)
                throw new InvalidOperationException("Right instance does not contain a collection of elements.");

            if (other["elements"] is not Instance[] otherElements)
                throw new InvalidOperationException("Left instance does not contain a collection of elements.");

            Instance[] newElements = [.. elements, .. otherElements];

            return new Instance(Collection(collectionType, collectionType.ElementType))
            {
                ["elements"] = CleanElements(newElements)
            };
        }

        public static Instance Contains(Instance collection, Instance value)
        {
            if (collection.Type is not CollectionType collectionType)
                throw new InvalidOperationException("Right instance is not a collection type.");

            if (collection["elements"] is not Instance[] elements)
                throw new InvalidOperationException("Right instance does not contain a collection of elements.");

            var contains = elements.Any(e => e.Equals(value));

            return BooleanType.Default.Construct(contains);
        }

        protected virtual Instance[] CleanElements(Instance[] elements)
        {
            return elements; // Sub class might do something with this array.
        }

    }

}
