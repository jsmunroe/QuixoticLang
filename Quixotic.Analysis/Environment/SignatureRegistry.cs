using Quixotic.Common.Symbols;
using Quixotic.Common.Types;
using System.Diagnostics.CodeAnalysis;

namespace Quixotic.Analysis.Environment
{
    public class SignatureRegistry
    {
        private readonly Dictionary<Signature, FunctionSignatureSymbol> _signatures = [];

        public void Register(string name, QxType returnType, params QxType[] parameters)
        {
            var signature = new Signature(name, parameters);

            var functionSignature = new FunctionSignatureSymbol(name, returnType, parameters);

            _signatures[signature] = functionSignature;
        }

        public bool Contains(string name, params QxType[] parameterTypes)
        {
            return TryResolve(name, parameterTypes, out _);
        }

        public FunctionSignatureSymbol? Resolve(string name, params QxType[] parameterTypes)
        {
            return TryResolve(name, parameterTypes, out var functionSymbol) ? functionSymbol : null;
        }

        public bool TryResolve(string name, QxType[] parameterTypes, [NotNullWhen(returnValue: true)] out FunctionSignatureSymbol? functionSignature)
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
    }
}
