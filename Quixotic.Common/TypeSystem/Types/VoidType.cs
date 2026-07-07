namespace Quixotic.Common.TypeSystem.Types
{
    public class VoidType : QxType
    {
        public static VoidType Default { get; } = new();
        public static Instance Value { get; } = new Instance(Default);

        protected VoidType() : base("void")
        { }
    }
}
