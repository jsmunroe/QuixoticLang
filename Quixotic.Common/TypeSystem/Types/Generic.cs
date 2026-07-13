namespace Quixotic.Common.TypeSystem.Types
{
    public class Generic(string key) : QxType($"<{key}>")
    {
        public string Key { get; } = key;

        public override bool HasGenerics => true;

        public override QxType Substitute(GenericBindings bindings)
        {
            return bindings.TryGet(Key, out var type) ? type : this;
        }

        public override bool Match(QxType actual, GenericBindings bindings)
        {
            return bindings.TryBind(Key, actual);
        }

        public override bool IsAssignableFrom(QxType other)
        {
            return true;
        }
    }
}
