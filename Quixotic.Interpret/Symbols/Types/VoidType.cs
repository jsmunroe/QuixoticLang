namespace Quixotic.Interpret.Symbols.Types
{
    public class VoidType : QxType
    {
        public static VoidType Instance { get; } = new();

        protected VoidType() : base("void")
        { }
    }
}
