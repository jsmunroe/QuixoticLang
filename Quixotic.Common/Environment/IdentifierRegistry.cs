using Quixotic.Common.Symbols;
using Quixotic.Common.Syntax.Casing;
using Quixotic.Common.TypeSystem.Types;
using System.Diagnostics.CodeAnalysis;

namespace Quixotic.Common.Environment
{
    public class IdentifierRegistry
    {
        public readonly Dictionary<string, IdentifierSymbol> _identifiers = new(CaseRule.Current.StringComparer);

        public List<IdentifierSymbol> AllIdentifiers => [.. _identifiers.Values];

        public void Add(IdentifierRegistry other)
        {
            foreach (var variable in other.AllIdentifiers)
                _identifiers[variable.Name] = new(variable);
        }

        public IdentifierRegistry Capture(ClosureCapture closureCapture)
        {
            var registry = new IdentifierRegistry();

            foreach (var variable in AllIdentifiers.Where(closureCapture.IsCaptured))
                registry.Register(variable);

            return registry;
        }

        public IdentifierSymbol Register(string name, QxType type, QxType? valueType)
        {
            var identifierSymbol = new IdentifierSymbol(name, type, valueType);
            _identifiers[name] = identifierSymbol;

            return identifierSymbol;
        }


        public IdentifierSymbol Register(IdentifierSymbol symbol)
        {
            _identifiers[symbol.Name] = symbol;
            return symbol;
        }

        public bool Contains(string name)
        {
            return _identifiers.ContainsKey(name);
        }

        public IdentifierSymbol? Resolve(string name)
        {
            return TryResolve(name, out var type) ? type : null;
        }

        public bool TryResolve(string name, [NotNullWhen(returnValue: true)] out IdentifierSymbol? variable)
        {
            return _identifiers.TryGetValue(name, out variable);
        }
    }
}
