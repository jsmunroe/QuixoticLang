using Quixotic.Common.Exceptions.Interpret;
using System.ComponentModel;

namespace Quixotic.Common.TypeSystem.Types
{
    [Description("array")]
    public class ArrayType(QxType elementType, ArrayDefinition definition) : CollectionType($"{elementType}[]", elementType, definition)
    {
        public Instance Construct(Instance[] elements)
        {
            var array = new Instance(this);

            Assign(array, elements);

            return array;
        }

        public override bool Match(QxType actual, GenericBindings bindings)
        {
            if (actual is not ArrayType collection)
                return false;

            return ElementType.Match(collection.ElementType, bindings);
        }

        public void Assign(Instance array, Instance[] elements)
        {
            if (!array.Type.Equals(this))
                throw new InvalidOperationException($"Instance is not of type {this}.");

            foreach (var element in elements)
            {
                if (!ElementType.IsAssignableFrom(element.Type))
                    throw new VariableTypeMismatchException(element.Type, ElementType);
            }

            array["elements"] = elements;
        }

        public Instance GetElement(Instance instance, int index)
        {
            if (instance["elements"] is not Instance[] elements)
                throw new InvalidOperationException("Instance does not contain an array of elements.");

            if (index < 0 || index >= elements.Length)
                throw new IndexOutOfRangeException($"Index {index} is out of bounds for array of length {(int)instance["length"]!}.");

            return elements[index];
        }

        public Instance GetElement(Instance instance, Instance index)
        {
            if (index["value"] is not double indexValue)
                throw new InvalidOperationException("Index instance does not contain a numeric value.");

            return GetElement(instance, (int)indexValue);
        }

        public void SetElement(int index, Instance instance, Instance value)
        {
            if (instance["elements"] is not Instance[] elements)
                throw new InvalidOperationException("Instance does not contain an array of elements.");

            if (!ElementType.IsAssignableFrom(value.Type))
                throw new InvalidOperationException($"Value is not of type {ElementType}.");

            elements[index] = value;
        }

        public void SetElement(Instance index, Instance instance, Instance value)
        {
            if (index["value"] is not double indexValue)
                throw new InvalidOperationException("Index instance does not contain a numeric value.");

            SetElement((int)indexValue, instance, value);
        }

        public override bool IsAssignableFrom(QxType other)
        {
            if (other is not ArrayType arrayType)
                return false;

            if (!ElementType.IsAssignableFrom(arrayType.ElementType))
                return false;

            return true;
        }

        public override bool IsTruthy(Instance instance)
        {
            if (instance["elements"] is Instance[] array)
            {
                return array.Length > 0;
            }

            return false;
        }

        public override string ToString(Instance instance)
        {
            var elements = (Instance[])instance["elements"]!;

            return $"[{string.Join(", ", elements.Select(e => e.ToString()))}]";
        }

    }
}
