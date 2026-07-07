using Quixotic.Common.TypeSystem.Symbols;
using System.ComponentModel;

namespace Quixotic.Common.TypeSystem.Types
{
    [Description("array")]
    public class ArrayType : QxType
    {
        public QxType ElementType { get; }

        public ArrayType(QxType elementType) : base($"{elementType}[]")
        {
            ElementType = elementType;

            RegisterMethod("length", (Instance instance) => Number.Construct(Length(instance)), Number, new Parameter("this", this));
        }

        public Instance Construct(Instance[] elements)
        {
            var array = new Instance(this);

            Assign(array, elements);

            return array;
        }

        public void Assign(Instance array, Instance[] elements)
        {
            if (!array.Type.Equals(this))
                throw new InvalidOperationException($"Instance is not of type {this}.");

            array["elements"] = elements;
        }

        public int Length(Instance instance)
        {
            if (instance["elements"] is not Instance[] elements)
                throw new InvalidOperationException("Instance does not contain an array of elements.");

            return elements.Length;
        }

        public Instance Get(Instance instance, int index)
        {
            if (instance["elements"] is not Instance[] elements)
                throw new InvalidOperationException("Instance does not contain an array of elements.");

            if (index < 0 || index >= elements.Length)
                throw new IndexOutOfRangeException($"Index {index} is out of bounds for array of length {(int)instance["length"]!}.");

            return elements[index];
        }

        public Instance Get(Instance instance, Instance index)
        {
            if (index["value"] is not double indexValue)
                throw new InvalidOperationException("Index instance does not contain a numeric value.");

            return Get(instance, (int)indexValue);
        }

        public void Set(int index, Instance instance, Instance value)
        {
            if (instance["elements"] is not Instance[] elements)
                throw new InvalidOperationException("Instance does not contain an array of elements.");

            if (!ElementType.IsAssignableFrom(value.Type))
                throw new InvalidOperationException($"Value is not of type {ElementType}.");

            elements[index] = value;
        }

        public void Set(Instance index, Instance instance, Instance value)
        {
            if (index["value"] is not double indexValue)
                throw new InvalidOperationException("Index instance does not contain a numeric value.");

            Set((int)indexValue, instance, value);
        }

        public Instance Append(Instance array, Instance value)
        {
            if (array.Type is not ArrayType arrayType)
                throw new InvalidOperationException("Right instance is not an array type.");

            if (array["elements"] is not Instance[] elements)
                throw new InvalidOperationException("Array instance does not contain an array of elements.");

            if (!arrayType.ElementType.IsAssignableFrom(value.Type))
                throw new InvalidOperationException($"Value is not of type {arrayType.ElementType}.");

            Instance[] newElements = [.. elements, value];

            return new Instance(Array(arrayType.ElementType))
            {
                ["elements"] = newElements
            };
        }

        public Instance Concat(Instance array, Instance other)
        {
            if (array.Type is not ArrayType arrayType)
                throw new InvalidOperationException("Right instance is not an array type.");

            if (other.Type is not ArrayType otherArrayType)
                throw new InvalidOperationException("Left instance is not an array type.");

            if (!arrayType.ElementType.IsAssignableFrom(otherArrayType.ElementType))
                throw new InvalidOperationException($"Elements of type {otherArrayType.ElementType} are not assignable to an array of type {arrayType.ElementType}");

            if (array["elements"] is not Instance[] elements)
                throw new InvalidOperationException("Right instance does not contain an array of elements.");

            if (other["elements"] is not Instance[] otherElements)
                throw new InvalidOperationException("Left instance does not contain an array of elements.");

            Instance[] newElements = [.. elements, .. otherElements];

            return new Instance(Array(arrayType.ElementType))
            {
                ["elements"] = newElements
            };
        }

        public override bool IsAssignableFrom(QxType subtype)
        {
            if (subtype is not ArrayType arrayType)
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
    }
}
