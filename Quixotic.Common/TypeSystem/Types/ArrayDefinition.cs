namespace Quixotic.Common.TypeSystem.Types
{
    public class ArrayDefinition() : CollectionDefinition("TItem[]")
    {
        public Dictionary<QxType, ArrayType> _arrayTypes = [];

        public override bool HasGenerics => true;

        public override QxType MakeGenericType(GenericBindings bindings)
        {
            if (!bindings.TryGet("TItem", out var elementType))
                throw new Exception("Type 'TItem' is not given.");

            if (_arrayTypes.TryGetValue(elementType, out var arrayType))
                return arrayType;

            arrayType = new ArrayType(elementType, this);

            _arrayTypes[elementType] = arrayType;

            return arrayType;
        }

        public override bool Match(QxType actual, GenericBindings bindings)
        {
            if (actual is ArrayDefinition)
                return true;

            if (actual is not ArrayType arrayType)
                return false;

            return ElementType.Match(arrayType.ElementType, bindings);
        }

        public override bool IsAssignableFrom(QxType other)
        {
            if (other is ArrayDefinition || other is ArrayType)
                return true;

            return base.IsAssignableFrom(other);
        }
    }
}
