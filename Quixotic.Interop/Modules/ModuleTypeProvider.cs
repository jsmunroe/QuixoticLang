using Quixotic.Common.Contracts;
using Quixotic.Common.Environment;
using Quixotic.Common.TypeSystem;
using Quixotic.Common.TypeSystem.Types;

namespace Quixotic.Interop.Modules
{
    public class ModuleTypeProvider(IEnumerable<QxType> types) : ITypeProvider
    {
        private readonly Dictionary<TypeName, QxType> _types = types.ToDictionary(t => t.Name);

        public ModuleTypeProvider()
            : this([])
        { }

        public void Register(TypeRegistry registry)
        {
            foreach (var type in _types.Values)
                registry.Register(type.Name, type);
        }

        public void Add(QxType type)
        {
            _types[type.Name] = type;
        }
    }
}
