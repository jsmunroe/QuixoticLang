using Quixotic.Common.Contracts;
using Quixotic.Common.Environment;
using System.Reflection;

namespace Quixotic.Common.Namespaces
{
    public class ImportSet(TypeRegistry? typeRegistry = null)
    {
        public readonly Dictionary<Namespace, IModule> _modules = []; // Case sensitivity is handled by Namespace

        public bool Load(string assemblyFile)
        {
            var assembly = Assembly.LoadFrom(assemblyFile);

            var moduleTypes = assembly.GetTypes().Where(t => typeof(IModule).IsAssignableFrom(t));

            foreach (var type in moduleTypes)
            {
                try
                {
                    var module = (IModule)Activator.CreateInstance(type)!;

                    Add(module);
                }
                catch (Exception)
                {
                    return false;
                }
            }

            return true;
        }

        public void Add(IModule module)
        {
            if (_modules.ContainsKey(module.Namespace))
                return;

            _modules.Add(module.Namespace, module);

            if (typeRegistry is not null)
                module.Types.Register(typeRegistry);
        }

        public void Register(TypeRegistry registry)
        {
            foreach (var module in _modules.Values)
                module.Types.Register(registry);
        }
    }


}
