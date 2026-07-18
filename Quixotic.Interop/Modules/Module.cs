using Quixotic.Common.Contracts;
using Quixotic.Common.Namespaces;
using Quixotic.Common.TypeSystem.Types;

namespace Quixotic.Interop.Modules
{
    public abstract class Module(Namespace @namespace) : IModule
    {
        public Namespace Namespace { get; } = @namespace;

        public ITypeProvider Types => GetTypeProvider();

        protected ModuleTypeProvider GetTypeProvider()
        {
            return new(GetTypes());
        }

        protected abstract IEnumerable<QxType> GetTypes();
    }
}
