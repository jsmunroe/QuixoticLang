using System.ComponentModel;

namespace Quixotic.Common.TypeSystem.Types
{
    [Description("boolean")]
    public class BooleanType : QxValueType
    {
        public static BooleanType Instance { get; } = new();

        protected BooleanType()
            : base("boolean", typeof(bool))
        { }

        public Instance Construct(bool value)
        {
            return new Instance(this)
            {
                ["value"] = value
            };
        }

        public bool Get(Instance instance)
        {
            if (instance["value"] is bool boolValue)
                return boolValue;

            return false;
        }

        public bool Set(Instance instance, bool value)
        {
            instance["value"] = value;
            return true;
        }

        public override bool IsTruthy(Instance instance)
        {
            if (instance["value"] is bool boolValue)
                return boolValue;

            return false;
        }
    }
}
