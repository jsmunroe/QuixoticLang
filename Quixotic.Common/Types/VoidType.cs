namespace Quixotic.Common.Types
{
    public class VoidType : QxType
    {
        public static VoidType Instance { get; } = new();

        protected VoidType() : base("void")
        { }
    }
}
