using Quixotic.Common.Namespaces;
using Quixotic.Common.TypeSystem.Types;
using System.Reflection;

namespace Quixotic.Interop.Modules
{
    public abstract class AssemblyModule(Assembly assembly, Namespace @namespace) : Module(@namespace)
    {
        protected override IEnumerable<QxType> GetTypes()
        {
            var typeLoader = ClrTypeLoader.FromAssembly(assembly);

            return typeLoader.Load();
        }
    }
}
