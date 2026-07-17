using Quixotic.Common.Contracts;
using Quixotic.Common.Exceptions.Interop;
using System.Diagnostics.CodeAnalysis;

namespace Quixotic.Common.Namespaces
{
    public class AssemblyCatalog
    {
        public readonly Dictionary<Namespace, string> _locations = []; // Namespace manages case-sensitivity.

        public AssemblyCatalog(string root)
        {
            Root = root;

            EnumerateAssemblies(root);
        }

        public string Root { get; }

        public List<Namespace> Namespaces => [.. _locations.Keys];

        public bool ContainsNamespace(Namespace ns)
        {
            return _locations.ContainsKey(ns);
        }

        public bool TryGetLocation(Namespace ns, [NotNullWhen(returnValue: true)] out string? location)
        {
            return _locations.TryGetValue(ns, out location);
        }

        private void EnumerateAssemblies(string root)
        {
            foreach (var file in Directory.EnumerateFiles(root, "*.dll"))
            {
                var ns = new Namespace(Path.GetFileNameWithoutExtension(file));
                _locations[ns] = file;
            }
        }

        public static AssemblyCatalog For(params IAssemblyLocator[] locators)
        {
            foreach (var locator in locators)
            {
                if (locator.TryLocate(out var assemblyRoot))
                    return new AssemblyCatalog(assemblyRoot);
            }

            throw new AssembliesNotFoundException();
        }
    }
}
