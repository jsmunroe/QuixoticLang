using Quixotic.Common.Contracts;
using Quixotic.Common.Environment;
using Quixotic.Common.Symbols;
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
            registry.Register("+", Wrap(Concat), QxType.String, FunctionCallType.OperatorCall, Param("left", QxType.String), Param("right", QxType.Any));
            registry.Register("+", Wrap(Add), QxType.Number, FunctionCallType.OperatorCall, Param("left", QxType.Number), Param("right", QxType.Number));
            registry.Register("-", Wrap(Subtract), QxType.Number, FunctionCallType.OperatorCall, Param("left", QxType.Number), Param("right", QxType.Number));
            registry.Register("*", Wrap(Multiply), QxType.Number, FunctionCallType.OperatorCall, Param("left", QxType.Number), Param("right", QxType.Number));
            registry.Register("/", Wrap(Divide), QxType.Number, FunctionCallType.OperatorCall, Param("left", QxType.Number), Param("right", QxType.Number));
            registry.Register("<", Wrap(IsLessThan), QxType.Boolean, FunctionCallType.OperatorCall, Param("left", QxType.Number), Param("right", QxType.Number));
            registry.Register("<=", Wrap(IsLessThanOrEqualTo), QxType.Boolean, FunctionCallType.OperatorCall, Param("left", QxType.Number), Param("right", QxType.Number));
            registry.Register(">", Wrap(IsGreaterThan), QxType.Boolean, FunctionCallType.OperatorCall, Param("left", QxType.Number), Param("right", QxType.Number));
            registry.Register(">=", Wrap(IsGreaterThanOrEqualTo), QxType.Boolean, FunctionCallType.OperatorCall, Param("left", QxType.Number), Param("right", QxType.Number));
            registry.Register("=", Wrap(IsEqualTo), QxType.Boolean, FunctionCallType.OperatorCall, Param("left", QxType.Any), Param("right", QxType.Any));
            registry.Register("!=", Wrap(IsNotEqualTo), QxType.Boolean, FunctionCallType.OperatorCall, Param("left", QxType.Any), Param("right", QxType.Any));
            registry.Register("-", Wrap(Negate), QxType.Number, FunctionCallType.OperatorCall, Param("left", QxType.Number));
            registry.Register("not", Wrap(Not), QxType.Boolean, FunctionCallType.OperatorCall, Param("left", QxType.Any));
            registry.Register("+", Wrap(CollectionAppend), QxType.Collection(QxType.Any), FunctionCallType.OperatorCall, Param("left", QxType.Collection(QxType.Any)), Param("right", QxType.Any));
            registry.Register("+", Wrap(CollectionConcat), QxType.Collection(QxType.Any), FunctionCallType.OperatorCall, Param("left", QxType.Collection(QxType.Any)), Param("right", QxType.Collection(QxType.Any)));
            registry.Register("in", Wrap(CollectionContains), QxType.Boolean, FunctionCallType.OperatorCall, Param("left", QxType.Any), Param("right", QxType.Collection(QxType.Any)));
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

        public static Instance Negate(NumberValue left)
        {
            return new NumberValue(-left.Value);
        }

        public static Instance Not(Instance left)
        {
            return new BooleanValue(!left.IsTruthy);
        }

        public static Instance CollectionAppend(CollectionReference array, Instance element)
        {
            if (array.Type is not CollectionType collectionType)
                throw new ArgumentException("Argument is not a CollectionType", nameof(array));

            return collectionType.Append(array, element);
        }

        public static Instance CollectionConcat(CollectionReference array, CollectionReference element)
        {
            if (array.Type is not CollectionType collectionType)
                throw new ArgumentException("Argument is not a CollectionType", nameof(array));

            return collectionType.Concat(array, element);
        }

        public static Instance CollectionContains(Instance element, CollectionReference array)
        {
            if (array.Type is not CollectionType collectionType)
                throw new ArgumentException("Argument is not a CollectionType", nameof(array));

            return collectionType.Contains(array, element);
        }

        private static ExternalFunction Wrap(Delegate del)
        {
            return (args) => del.DynamicInvoke(args) as Instance ?? QxType.Void;
        }

        private static Parameter Param(string name, QxType type) => new(name, type);
    }
}
