using Quixotic.Common.TypeSystem.Symbols;
using System.ComponentModel;
using System.Diagnostics;

namespace Quixotic.Common.TypeSystem.Types
{
    [Description("number")]
    public class NumberType : QxValueType
    {
        public static NumberType Default { get; } = new();

        protected NumberType()
            : base("number", typeof(double))
        { }

        public Instance Construct(double value)
        {
            return new Instance(this)
            {
                ["value"] = value
            };
        }

        protected override void LoadMethods()
        {
            RegisterStaticMethod("+", Add, Number, new Parameter("left", Number), new Parameter("right", Number));
            RegisterStaticMethod("-", Subtract, Number, new Parameter("left", Number), new Parameter("right", Number));
            RegisterStaticMethod("*", Multiply, Number, new Parameter("left", Number), new Parameter("right", Number));
            RegisterStaticMethod("/", Divide, Number, new Parameter("left", Number), new Parameter("right", Number));
            RegisterStaticMethod("<", IsLessThan, Boolean, new Parameter("left", Number), new Parameter("right", Number));
            RegisterStaticMethod("<=", IsLessThanOrEqualTo, Boolean, new Parameter("left", Number), new Parameter("right", Number));
            RegisterStaticMethod(">", IsGreaterThan, Boolean, new Parameter("left", Number), new Parameter("right", Number));
            RegisterStaticMethod(">=", IsGreaterThanOrEqualTo, Boolean, new Parameter("left", Number), new Parameter("right", Number));
            RegisterStaticMethod("-", Negate, Number, new Parameter("left", Number));
        }

        public double Get(Instance instance)
        {
            return instance["value"] is double d ? d : 0;
        }

        public override bool IsTruthy(Instance instance)
        {
            return instance["value"] is double d && d != 0;
        }

        public override bool Match(QxType actual, GenericBindings bindings)
        {
            return actual is NumberType;
        }

        private Instance Add(params Instance[] args)
        {
            Debug.Assert(args.Length == 2);

            var left = args[0];
            var right = args[1];

            return Construct(Get(left) + Get(right));
        }

        private Instance Subtract(params Instance[] args)
        {
            Debug.Assert(args.Length == 2);

            var left = args[0];
            var right = args[1];

            return Construct(Get(left) - Get(right));
        }

        private Instance Multiply(params Instance[] args)
        {
            Debug.Assert(args.Length == 2);

            var left = args[0];
            var right = args[1];

            return Construct(Get(left) * Get(right));
        }

        private Instance Divide(params Instance[] args)
        {
            Debug.Assert(args.Length == 2);

            var left = args[0];
            var right = args[1];

            return Construct(Get(left) / Get(right));
        }

        private Instance IsLessThan(params Instance[] args)
        {
            Debug.Assert(args.Length == 2);

            var left = args[0];
            var right = args[1];

            return BooleanType.Default.Construct(Get(left) < Get(right));
        }

        private Instance IsLessThanOrEqualTo(params Instance[] args)
        {
            Debug.Assert(args.Length == 2);

            var left = args[0];
            var right = args[1];

            return BooleanType.Default.Construct(Get(left) <= Get(right));
        }

        private Instance IsGreaterThan(params Instance[] args)
        {
            Debug.Assert(args.Length == 2);

            var left = args[0];
            var right = args[1];

            return BooleanType.Default.Construct(Get(left) > Get(right));
        }

        private Instance IsGreaterThanOrEqualTo(params Instance[] args)
        {
            Debug.Assert(args.Length == 2);

            var left = args[0];
            var right = args[1];

            return BooleanType.Default.Construct(Get(left) >= Get(right));
        }

        private Instance Negate(params Instance[] args)
        {
            Debug.Assert(args.Length == 1);

            var operand = args[0];

            return Construct(-Get(operand));
        }
    }
}
