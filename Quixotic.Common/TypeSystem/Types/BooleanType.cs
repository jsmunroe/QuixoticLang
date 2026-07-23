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

        protected override void LoadMethods()
        {
            base.LoadMethods();
        }

        public Instance Construct(bool value)
        {
            return new Instance(Default)
            {
                ["value"] = value
            };
        }

        public bool Get(Instance instance)
        {
            return instance["value"] is bool b && b;
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

        public static Instance True { get; } = Default.Construct(true);

        public static Instance False { get; } = Default.Construct(false);

    }
}
