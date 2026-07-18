using Quixotic.Common.Contracts;
using System.Diagnostics.CodeAnalysis;

namespace Quixotic.Common.Namespaces
{
    public class DevelopmentLibraryLocator : IAssemblyLocator
    {
        private static string? _assemblyRoot = null;

        public bool TryLocate([NotNullWhen(returnValue: true)] out string? assemblyRoot)
        {
            assemblyRoot = null;
#if DEBUG
            var root = Path.Combine(GetSolutionDirectory(), "Libraries");

            if (Directory.Exists(root))
            {
                assemblyRoot = root;
                return true;
            }

            assemblyRoot = null;
#endif
            return false;
        }

        private static string GetSolutionDirectory()
        {
            if (_assemblyRoot is not null)
                return _assemblyRoot;

            var currentDir = new DirectoryInfo(AppContext.BaseDirectory);

            while (currentDir != null)
            {
                // Check if any file in the current directory ends with .sln
                if (currentDir.GetFiles("*.sln*").Any())
                {
                    _assemblyRoot = currentDir.FullName;
                    return currentDir.FullName;
                }
                currentDir = currentDir.Parent;
            }

            throw new FileNotFoundException("Solution directory could not be determined.");
        }
    }
}
