using System.Diagnostics.CodeAnalysis;

namespace Quixotic.Common.Contracts
{
    public interface IAssemblyLocator
    {
        bool TryLocate([NotNullWhen(returnValue: true)] out string? assemblyRoot);
    }
}
