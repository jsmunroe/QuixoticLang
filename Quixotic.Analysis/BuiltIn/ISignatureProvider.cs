using Quixotic.Analysis.Environment;

namespace Quixotic.Analysis.BuiltIn
{
    public interface ISignatureProvider
    {
        void Register(SignatureRegistry registry);
    }
}