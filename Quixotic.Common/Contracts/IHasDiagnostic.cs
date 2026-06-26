using Quixotic.Common.Diagnostics;

namespace Quixotic.Common.Contracts
{
    public interface IHasDiagnostic
    {
        Diagnostic Diagnostic { get; }
    }
}
