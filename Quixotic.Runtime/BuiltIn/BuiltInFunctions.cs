using Quixotic.Common.Contracts;
using Quixotic.Common.Environment;
using Quixotic.Common.TypeSystem;
using Quixotic.Common.TypeSystem.Symbols;
using Quixotic.Common.TypeSystem.Types;
using Quixotic.Runtime.References;
using Quixotic.Runtime.Values;

namespace Quixotic.Runtime.BuiltIn
{
    public class BuiltInFunctions : IFunctionProvider
    {
        public void Register(FunctionRegistry registry)
        {
            registry.Register("+", Concat, QxType.String, Param("left", QxType.String), Param("right", QxType.Any));
            registry.Register("+", Add, QxType.Number, Param("left", QxType.Number), Param("right", QxType.Number));
            registry.Register("-", Subtract, QxType.Number, Param("left", QxType.Number), Param("right", QxType.Number));
            registry.Register("*", Multiply, QxType.Number, Param("left", QxType.Number), Param("right", QxType.Number));
            registry.Register("/", Divide, QxType.Number, Param("left", QxType.Number), Param("right", QxType.Number));
            registry.Register("<", IsLessThan, QxType.Boolean, Param("left", QxType.Number), Param("right", QxType.Number));
            registry.Register("<=", IsLessThanOrEqualTo, QxType.Boolean, Param("left", QxType.Number), Param("right", QxType.Number));
            registry.Register(">", IsGreaterThan, QxType.Boolean, Param("left", QxType.Number), Param("right", QxType.Number));
            registry.Register(">=", IsGreaterThanOrEqualTo, QxType.Boolean, Param("left", QxType.Number), Param("right", QxType.Number));
            registry.Register("=", IsEqualTo, QxType.Boolean, Param("left", QxType.Any), Param("right", QxType.Any));
            registry.Register("!=", IsNotEqualTo, QxType.Boolean, Param("left", QxType.Any), Param("right", QxType.Any));
            registry.Register("+", ArrayAppend, QxType.Collection(QxType.Any), Param("left", QxType.Collection(QxType.Any)), Param("right", QxType.Any));
            registry.Register("+", ArrayConcat, QxType.Collection(QxType.Any), Param("left", QxType.Collection(QxType.Any)), Param("right", QxType.Collection(QxType.Any)));
            registry.Register("in", Contains, QxType.Boolean, Param("left", QxType.Any), Param("right", QxType.Collection(QxType.Any)));
        }

        public static Instance Concat(StringValue left, Instance right)
        {
            return new StringValue($"{left.Value}{right}");
        }

        public static Instance Add(NumberValue left, NumberValue right)
        {
            return new NumberValue(left.Value + right.Value);
        }

        public static Instance Subtract(NumberValue left, NumberValue right)
        {
            return new NumberValue(left.Value - right.Value);
        }

        public static Instance Multiply(NumberValue left, NumberValue right)
        {
            return new NumberValue(left.Value * right.Value);
        }

        public static Instance Divide(NumberValue left, NumberValue right)
        {
            return new NumberValue(left.Value / right.Value);
        }

        public static Instance IsLessThan(NumberValue left, NumberValue right)
        {
            return new BooleanValue(left.Value < right.Value);
        }

        public static Instance IsLessThanOrEqualTo(NumberValue left, NumberValue right)
        {
            return new BooleanValue(left.Value <= right.Value);
        }

        public static Instance IsGreaterThan(NumberValue left, NumberValue right)
        {
            return new BooleanValue(left.Value > right.Value);
        }

        public static Instance IsGreaterThanOrEqualTo(NumberValue left, NumberValue right)
        {
            return new BooleanValue(left.Value >= right.Value);
        }

        public static Instance IsEqualTo(Instance left, Instance right)
        {
            return new BooleanValue(left.Equals(right));
        }

        public static Instance IsNotEqualTo(Instance left, Instance right)
        {
            return new BooleanValue(!left.Equals(right));
        }

        public static Instance ArrayAppend(CollectionReference array, Instance element)
        {
            if (array.Type is not CollectionType collectionType)
                throw new ArgumentException("Argument is not a CollectionType", nameof(array));

            return collectionType.Append(array, element);
        }

        public static Instance ArrayConcat(CollectionReference array, CollectionReference element)
        {
            if (array.Type is not CollectionType collectionType)
                throw new ArgumentException("Argument is not a CollectionType", nameof(array));

            return collectionType.Concat(array, element);
        }

        public static Instance Contains(Instance element, CollectionReference array)
        {
            if (array.Type is not CollectionType collectionType)
                throw new ArgumentException("Argument is not a CollectionType", nameof(array));

            return collectionType.Contains(array, element);
        }

        private static Parameter Param(string name, QxType type) => new(name, type);
    }
}
