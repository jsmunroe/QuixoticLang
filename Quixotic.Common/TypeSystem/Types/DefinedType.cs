namespace Quixotic.Common.TypeSystem.Types
{
    public class DefinedType(string name) : QxType(name)
    {
        public override Instance Construct()
        {
            return base.Construct();
        }
    }
}
