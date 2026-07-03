using Quixotic.Analysis.Environment;

namespace Quixotic.Analysis.Contracts
{
    public interface ISignatureProvider
    {
        void Register(SignatureRegistry registry);
    }
}