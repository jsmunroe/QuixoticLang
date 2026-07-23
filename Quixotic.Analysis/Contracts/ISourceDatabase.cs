using System.Diagnostics.CodeAnalysis;

namespace Quixotic.Analysis.Contracts
{
    public interface ISourceDatabase
    {
        ISourceDatabaseEntry? Query(int index);
        ISourceDatabaseEntry? Query(int lineNumber, int column);
        bool TryQuery(int lineNumber, int column, [NotNullWhen(true)] out ISourceDatabaseEntry? entry);
        bool TryQuery(int index, [NotNullWhen(true)] out ISourceDatabaseEntry? entry);
    }
}