using Quixotic.Analysis.Contracts;
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
            registry.Register("and", QxType.Boolean, QxType.Any, QxType.Any);
            registry.Register("or", QxType.Boolean, QxType.Any, QxType.Any);

            var tElement = QxType.Generic("TElement");
            registry.Register("+", QxType.Array(tElement), QxType.Array(tElement), tElement);

            tElement = QxType.Generic("TElement");
            registry.Register("+", QxType.Array(tElement), QxType.Array(tElement), QxType.Array(tElement));
        }
    }
}
