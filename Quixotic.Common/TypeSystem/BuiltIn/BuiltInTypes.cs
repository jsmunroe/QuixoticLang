using Quixotic.Common.Contracts;
using Quixotic.Common.Environment;
using Quixotic.Common.TypeSystem.Types;

namespace Quixotic.Common.TypeSystem.BuiltIn
{
    public class BuiltInTypes : ITypeProvider
    {
        public void Register(TypeRegistry registry)
        {
            registry.Register("any", Any);
            registry.Register("void", Void.Type);
            registry.Register("number", Number);
            registry.Register("string", String);
            registry.Register("boolean", Boolean);
            registry.Register("function", Function);
            registry.Register("dynamic", new DynamicType());
            registry.Register("{TItem}[]", Array);
            registry.Register("set<{TItem}>", Set);
        }

        public static QxMetaType Meta(QxType typeReference) => new(typeReference);

        public static QxType Any { get; } = AnyType.Default;

        public static NumberType Number { get; } = NumberType.Default;
        public static StringType String { get; } = StringType.Default;
        public static BooleanType Boolean { get; } = BooleanType.Default;
        public static Instance Nada { get; } = NadaType.Value;
        public static Instance Void { get; } = VoidType.Value;

        public static FunctionDefinition Function { get; } = FunctionDefinition.Default;

        public static ArrayDefinition Array { get; } = new();
        public static SetDefinition Set { get; } = new();
        public static CollectionType Collection(CollectionType collectionType, QxType elementType)
        {
            return collectionType switch
            {
                ArrayType => Array.MakeGenericType(elementType),
                SetType => Set.MakeGenericType(elementType),
                _ => throw new InvalidOperationException($"Unsupported collection type: {collectionType.Name}")
            };
        }


        public static Generic Generic(string name) => new(name);

    }
}
