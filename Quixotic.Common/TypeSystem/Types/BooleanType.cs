using System.ComponentModel;

namespace Quixotic.Common.TypeSystem.Types
{
    [Description("boolean")]
    public class BooleanType : QxValueType
    {
        public static BooleanType Default { get; } = new();

        protected BooleanType()
            : base("boolean", typeof(bool))
        { }

        public static Instance Construct(bool value)
        {
            return new Instance(Default)
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

        public override bool Match(QxType actual, GenericBindings bindings)
        {
            return actual is BooleanType;
        }

        public static Instance True { get; } = Construct(true);

        public static Instance False { get; } = Construct(false);

    }
}
