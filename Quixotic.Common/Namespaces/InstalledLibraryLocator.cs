using Quixotic.Common.Contracts;
using System.Diagnostics.CodeAnalysis;

namespace Quixotic.Common.Namespaces
{
    public class InstalledLibraryLocator : IAssemblyLocator
    {
        public bool TryLocate([NotNullWhen(returnValue: true)] out string? assemblyRoot)
        {
            var root = Path.Combine(AppContext.BaseDirectory, "Libraries");

            if (Directory.Exists(root))
            {
                assemblyRoot = root;
                return true;
            }

            assemblyRoot = null;
            return false;
        }
    }
}
