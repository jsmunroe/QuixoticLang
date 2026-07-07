using Quixotic.Common.Symbols;
using Quixotic.Common.TypeSystem.Types;
using System.Diagnostics.CodeAnalysis;

namespace Quixotic.Analysis.Environment
{
    public class SignatureRegistry
    {
        private readonly Dictionary<Signature, SignatureSymbol> _signatures = [];

        public void Register(string name, QxType returnType, params QxType[] parameters)
        {
            var signatureSymbol = new SignatureSymbol(name, returnType, parameters);

            _signatures[signatureSymbol.Signature] = signatureSymbol;
        }

        public bool Contains(string name, params QxType[] parameterTypes)
        {
            return TryResolve(name, parameterTypes, out _);
        }

        public SignatureSymbol? Resolve(string name, params QxType[] parameterTypes)
        {
            return TryResolve(name, parameterTypes, out var functionSymbol) ? functionSymbol : null;
        }

        public SignatureSymbol? Resolve(Signature signature)
        {
            return TryResolve(signature.Name, [.. signature.Parameters], out var functionSymbol) ? functionSymbol : null;
        }

        public bool TryResolve(string name, QxType[] parameterTypes, [NotNullWhen(returnValue: true)] out SignatureSymbol? functionSignature)
        {
            functionSignature = null;

            var signature = new Signature(name, [.. parameterTypes]);

            if (_signatures.TryGetValue(signature, out functionSignature))
                return true;

            foreach (var (s, f) in _signatures)
            {

                if (s.IsCompatible(name, parameterTypes))
                {
                    functionSignature = f;
                    return true;
                }
            }

            return false;
        }


        public bool TryResolve(Signature signature, [NotNullWhen(returnValue: true)] out SignatureSymbol? functionSignature)
        {
            return TryResolve(signature.Name, [.. signature.Parameters], out functionSignature);
        }
    }
}
