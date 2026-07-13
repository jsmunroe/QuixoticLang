namespace Quixotic.Common.TypeSystem.Types
{
    public class VoidType : QxType
    {
        public static VoidType Default { get; } = new();
        public static Instance Value { get; } = new Instance(Default);

        protected VoidType() : base("void")
        { }

        public override bool HasGenerics => false;


        public override bool Match(QxType actual, GenericBindings bindings)
        {
            return actual is VoidType;
        }
    }
}
