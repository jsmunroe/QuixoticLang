using System.ComponentModel;

namespace Quixotic.Common.TypeSystem.Types
{
    [Description("any")]
    public class AnyType : QxType
    {
        public static AnyType Default { get; } = new();
        public static Instance Value { get; } = new Instance(Default);

        public override bool HasGenerics => false;


        protected AnyType() : base("any")
        { }

        public override bool IsAssignableFrom(QxType other)
        {
            return true;
        }

        public override bool Match(QxType actual, GenericBindings bindings)
        {
            return true;
        }
    }
}
