using System.ComponentModel;

namespace Quixotic.Common.TypeSystem.Types
{
    [Description("number")]
    public class NumberType : QxValueType
    {
        public static NumberType Instance { get; } = new();

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

        public override bool IsTruthy(Instance instance)
        {
            return instance["value"] is double d && d != 0;
        }
    }
}
