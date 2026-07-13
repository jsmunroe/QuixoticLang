using Quixotic.Common.TypeSystem;
using Quixotic.Common.TypeSystem.Symbols;
using Quixotic.Common.TypeSystem.Types;
using System.Diagnostics.CodeAnalysis;

namespace Quixotic.Common.Environment
{
    public class VariableRegistry
    {
        private readonly Dictionary<string, VariableSymbol> _variables = new(StringComparer.OrdinalIgnoreCase);

        public List<VariableSymbol> AllVariables => [.. _variables.Values];

        public void Add(VariableRegistry other)
        {
            foreach (var variable in other.AllVariables)
                _variables[variable.Name] = new(variable);
        }

        public void Register(string name, QxType type)
        {
            _variables[name] = new VariableSymbol(name, type);
        }

        public void Register(string name, Instance value)
        {
            _variables[name] = new VariableSymbol(name, value);
        }

        public bool Contains(string name)
        {
            return _variables.ContainsKey(name);
        }

        public VariableSymbol? Resolve(string name)
        {
            return TryResolve(name, out var type) ? type : null;
        }

        public bool TryResolve(string name, [NotNullWhen(returnValue: true)] out VariableSymbol? variable)
        {
            return _variables.TryGetValue(name, out variable);
        }
    }
}
