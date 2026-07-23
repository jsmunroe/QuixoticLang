namespace Quixotic.Common.TypeSystem.Types
{

    public abstract class GenericTypeDefinition(string name) : TypeDefinition(name)
    {
        public abstract QxType MakeGenericType(GenericBindings bindings);
    }

}
