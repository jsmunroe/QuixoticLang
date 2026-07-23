using Quixotic.Common.Contracts;
using Quixotic.Common.Environment;
using Quixotic.Common.Symbols;
using Quixotic.Common.TypeSystem.Symbols;
using Quixotic.Common.TypeSystem.Types;

namespace Quixotic.Common.TypeSystem.BuiltIn
{
    public class BuiltInFunctions : IFunctionProvider
    {
        public void Register(FunctionRegistry registry)
        {
            registry.Register("+", Wrap(Concat), QxType.String, CallType.OperatorCall, Param("left", QxType.String), Param("right", QxType.Any));
            registry.Register("+", Wrap(Add), QxType.Number, CallType.OperatorCall, Param("left", QxType.Number), Param("right", QxType.Number));
            registry.Register("-", Wrap(Subtract), QxType.Number, CallType.OperatorCall, Param("left", QxType.Number), Param("right", QxType.Number));
            registry.Register("*", Wrap(Multiply), QxType.Number, CallType.OperatorCall, Param("left", QxType.Number), Param("right", QxType.Number));
            registry.Register("/", Wrap(Divide), QxType.Number, CallType.OperatorCall, Param("left", QxType.Number), Param("right", QxType.Number));
            registry.Register("<", Wrap(IsLessThan), QxType.Boolean, CallType.OperatorCall, Param("left", QxType.Number), Param("right", QxType.Number));
            registry.Register("<=", Wrap(IsLessThanOrEqualTo), QxType.Boolean, CallType.OperatorCall, Param("left", QxType.Number), Param("right", QxType.Number));
            registry.Register(">", Wrap(IsGreaterThan), QxType.Boolean, CallType.OperatorCall, Param("left", QxType.Number), Param("right", QxType.Number));
            registry.Register(">=", Wrap(IsGreaterThanOrEqualTo), QxType.Boolean, CallType.OperatorCall, Param("left", QxType.Number), Param("right", QxType.Number));
            registry.Register("=", Wrap(IsEqualTo), QxType.Boolean, CallType.OperatorCall, Param("left", QxType.Any), Param("right", QxType.Any));
            registry.Register("!=", Wrap(IsNotEqualTo), QxType.Boolean, CallType.OperatorCall, Param("left", QxType.Any), Param("right", QxType.Any));
            registry.Register("-", Wrap(Negate), QxType.Number, CallType.OperatorCall, Param("left", QxType.Number));
            registry.Register("not", Wrap(Not), QxType.Boolean, CallType.OperatorCall, Param("left", QxType.Any));
        }

        public static Instance Concat(Instance left, Instance right)
        {
            var stringType = StringType.Default;

            return stringType.Construct($"{stringType.Get(left)}{right}");
        }

        public static Instance Add(Instance left, Instance right)
        {
            var numberType = NumberType.Default;

            return numberType.Construct(numberType.Get(left) + numberType.Get(right));
        }

        public static Instance Subtract(Instance left, Instance right)
        {
            var numberType = NumberType.Default;

            return numberType.Construct(numberType.Get(left) - numberType.Get(right));
        }

        public static Instance Multiply(Instance left, Instance right)
        {
            var numberType = NumberType.Default;

            return numberType.Construct(numberType.Get(left) * numberType.Get(right));
        }

        public static Instance Divide(Instance left, Instance right)
        {
            var numberType = NumberType.Default;

            return numberType.Construct(numberType.Get(left) / numberType.Get(right));
        }

        public static Instance IsLessThan(Instance left, Instance right)
        {
            var numberType = NumberType.Default;
            var booleanType = BooleanType.Default;

            return booleanType.Construct(numberType.Get(left) < numberType.Get(right));
        }

        public static Instance IsLessThanOrEqualTo(Instance left, Instance right)
        {
            var numberType = NumberType.Default;
            var booleanType = BooleanType.Default;

            return booleanType.Construct(numberType.Get(left) <= numberType.Get(right));
        }

        public static Instance IsGreaterThan(Instance left, Instance right)
        {
            var numberType = NumberType.Default;
            var booleanType = BooleanType.Default;

            return booleanType.Construct(numberType.Get(left) > numberType.Get(right));
        }

        public static Instance IsGreaterThanOrEqualTo(Instance left, Instance right)
        {
            var numberType = NumberType.Default;
            var booleanType = BooleanType.Default;

            return booleanType.Construct(numberType.Get(left) >= numberType.Get(right));
        }

        public static Instance IsEqualTo(Instance left, Instance right)
        {
            var booleanType = BooleanType.Default;

            return booleanType.Construct(left.Equals(right));
        }

        public static Instance IsNotEqualTo(Instance left, Instance right)
        {
            var booleanType = BooleanType.Default;

            return booleanType.Construct(!left.Equals(right));
        }

        public static Instance Negate(Instance left)
        {
            var numberType = NumberType.Default;

            return numberType.Construct(-numberType.Get(left));
        }

        public static Instance Not(Instance left)
        {
            var booleanType = BooleanType.Default;

            return booleanType.Construct(!booleanType.Get(left));
        }


        private static ExternalFunction Wrap(Delegate del)
        {
            return (args) => del.DynamicInvoke(args) as Instance ?? QxType.Void;
        }

        private static Parameter Param(string name, QxType type) => new(name, type);
    }
}
