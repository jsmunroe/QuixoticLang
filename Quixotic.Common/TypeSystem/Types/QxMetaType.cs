namespace Quixotic.Common.TypeSystem.Types
{
    public class QxMetaType(QxType typeReference) : QxType("type")
    {
        public override bool HasGenerics => false;

        public QxType TypeReference { get; } = typeReference;

        public override Instance Construct()
        {
            var instance = new Instance(this);
            instance["typeReference"] = TypeReference;
            return instance;
        }

        public override bool Match(QxType actual, GenericBindings bindings)
        {
            return actual is QxMetaType metaType && TypeReference.Equals(metaType.TypeReference);
        }
    }
}
