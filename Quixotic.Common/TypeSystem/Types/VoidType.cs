namespace Quixotic.Common.TypeSystem.Types
{
    public class VoidType : QxType
    {
        public static VoidType Instance { get; } = new();

        protected VoidType() : base("void")
        { }
    }
}
