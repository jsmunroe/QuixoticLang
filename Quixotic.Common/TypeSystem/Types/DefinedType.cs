namespace Quixotic.Common.TypeSystem.Types
{
    public class DefinedType(string name, QxType? baseType) : QxType(name, baseType)
    {
        public override bool HasGenerics => false;

        public override Instance Construct()
        {
            return base.Construct();
        }

        public override bool Match(QxType actual, GenericBindings bindings)
        {
            return actual is DefinedType other && Name.Equals(other.Name);
        }
    }
}
