using Quixotic.Common.TypeSystem.Types;
using System.Diagnostics.CodeAnalysis;

namespace Quixotic.Common.Contracts
{
    public interface ITypeRegistry
    {
        QxType? Resolve(string name);
        bool TryResolve(string name, [NotNullWhen(true)] out QxType? type);
    }
}