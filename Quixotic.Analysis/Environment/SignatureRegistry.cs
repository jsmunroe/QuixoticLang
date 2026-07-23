using Quixotic.Common.Environment;
using Quixotic.Common.Symbols;
using Quixotic.Common.TypeSystem;
using Quixotic.Common.TypeSystem.Types;
using System.Diagnostics.CodeAnalysis;

namespace Quixotic.Analysis.Environment
{
    public class SignatureRegistry
    {
        private readonly Dictionary<Signature, SignatureSymbol> _signatures = [];

        public IEnumerable<SignatureSymbol> AllSignatures => _signatures.Values;

        public SignatureRegistry Capture(ClosureCapture closureCapture)
        {
            var registry = new SignatureRegistry();

            foreach (var signature in AllSignatures.Where(closureCapture.IsCaptured))
                registry.Register(signature);

            return registry;
        }

        public SignatureSymbol Register(string name, QxType returnType, QxType[] parameters, CallType callType)
        {
            var signatureSymbol = new SignatureSymbol(name, returnType, parameters, callType);

            _signatures[signatureSymbol.Signature] = signatureSymbol;

            return signatureSymbol;
        }

        public SignatureSymbol Register(Signature signature, QxType returnType, CallType callType)
        {
            var signatureSymbol = new SignatureSymbol(signature, returnType, callType);

            _signatures[signature] = signatureSymbol;

            return signatureSymbol;
        }

        public SignatureSymbol Register(SignatureSymbol signatureSymbol)
        {
            _signatures[signatureSymbol.Signature] = signatureSymbol;

            return signatureSymbol;
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
                var bindings = new GenericBindings();

                if (s.TryMatch(name, parameterTypes, bindings))
                {
                    functionSignature = f.Substitute(bindings);

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
