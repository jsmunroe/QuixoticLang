using Quixotic.Common.Contracts;
using System.Diagnostics.CodeAnalysis;

namespace Quixotic.Common.Namespaces
{
    public class DevelopmentLibraryLocator : IAssemblyLocator
    {
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
            var currentDir = new DirectoryInfo(AppContext.BaseDirectory);

            while (currentDir != null)
            {
                // Check if any file in the current directory ends with .sln
                if (currentDir.GetFiles("*.sln*").Any())
                {
                    return currentDir.FullName;
                }
                currentDir = currentDir.Parent;
            }

            throw new FileNotFoundException("Solution directory could not be determined.");
        }
    }
}
