using System.ComponentModel;

namespace Quixotic.Common.TypeSystem.Types
{
    [Description("string")]
    public class StringType : QxValueType
    {
        public static StringType Default { get; } = new();

        protected StringType()
            : base("string", typeof(string))
        { }

        public Instance Construct(string value)
        {
            return new Instance(this)
            {
                ["value"] = value
            };
        }

        public override bool IsTruthy(Instance instance)
        {
            return !string.IsNullOrEmpty(instance["value"] as string);
        }

        public override bool Match(QxType actual, GenericBindings bindings)
        {
            return actual is StringType;
        }

    }
}
