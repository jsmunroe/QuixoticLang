using Quixotic.Analysis.Environment;
using Quixotic.Common.Types;

namespace Quixotic.Analysis.BuiltIn
{
    public class BuiltInSignatures : ISignatureProvider
    {
        public void Register(SignatureRegistry registry)
        {
            registry.Register("+", QxType.String, QxType.String, QxType.Any);
            registry.Register("+", QxType.Number, QxType.Number, QxType.Number);
            registry.Register("-", QxType.Number, QxType.Number, QxType.Number);
            registry.Register("*", QxType.Number, QxType.Number, QxType.Number);
            registry.Register("/", QxType.Number, QxType.Number, QxType.Number);
            registry.Register("<", QxType.Boolean, QxType.Number, QxType.Number);
            registry.Register("<=", QxType.Boolean, QxType.Number, QxType.Number);
            registry.Register(">", QxType.Boolean, QxType.Number, QxType.Number);
            registry.Register(">=", QxType.Boolean, QxType.Number, QxType.Number);
            registry.Register("=", QxType.Boolean, QxType.Any, QxType.Any);
            registry.Register("!=", QxType.Boolean, QxType.Any, QxType.Any);
        }
    }
}
