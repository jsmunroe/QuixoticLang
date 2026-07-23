namespace Quixotic.Common.TypeSystem.Types
{
    public class SetDefinition() : CollectionDefinition("set<TItem>")
    {
        public Dictionary<QxType, SetType> _setTypes = [];

        public override bool HasGenerics => true;

        public override QxType MakeGenericType(GenericBindings bindings)
        {
            if (!bindings.TryGet("TItem", out var elementType))
                throw new Exception("Type 'TItem' is not given.");

            if (_setTypes.TryGetValue(elementType, out var setType))
                return setType;

            setType = new SetType(elementType, this);

            _setTypes[elementType] = setType;

            return setType;
        }

        public override bool Match(QxType actual, GenericBindings bindings)
        {
            if (actual is SetDefinition)
                return true;

            if (actual is not SetType setType)
                return false;

            return ElementType.Match(setType.ElementType, bindings);
        }

        public override bool IsAssignableFrom(QxType other)
        {
            if (other is SetDefinition || other is SetType)
                return true;

            return base.IsAssignableFrom(other);
        }

        protected override Instance[] CleanElements(Instance[] elements)
        {
            return [.. elements.Distinct().OrderBy(e => Random.Shared.Next())];
        }
    }

}
