using Quixotic.Common.TypeSystem.Symbols;
using System.ComponentModel;
using System.Diagnostics;

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

        protected override void LoadMethods()
        {
            RegisterStaticMethod("+", Add, Number, new Parameter("left", String), new Parameter("right", Any));
        }

        private Instance Add(params Instance[] args)
        {
            Debug.Assert(args.Length == 2);

            var left = args[0];
            var right = args[1];

            return Construct(Get(left) + right.ToString());
        }

        public string Get(Instance instance)
        {
            return instance["value"] is string s ? s : string.Empty;
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
