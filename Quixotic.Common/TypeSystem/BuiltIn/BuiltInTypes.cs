using Quixotic.Common.Contracts;
using Quixotic.Common.Environment;
using Quixotic.Common.TypeSystem.Types;

namespace Quixotic.Common.TypeSystem.BuiltIn
{
    public class BuiltInTypes : ITypeProvider
    {
        public void Register(TypeRegistry registry)
        {
            registry.Register("any", QxType.Any);
            registry.Register("void", QxType.Void.Type);
            registry.Register("number", QxType.Number);
            registry.Register("string", QxType.String);
            registry.Register("boolean", QxType.Boolean);
            registry.Register("dynamic", new DynamicType());
            registry.Register("{element}[]", QxType.Array(QxType.Generic("element")));
            registry.Register("set<{element}>", QxType.Set(QxType.Generic("element")));
        }
    }
}
