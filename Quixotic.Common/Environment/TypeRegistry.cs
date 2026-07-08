using Quixotic.Common.TypeSystem.Symbols;
using Quixotic.Common.TypeSystem.Types;
using System.Diagnostics.CodeAnalysis;

namespace Quixotic.Common.Environment
{
    public class TypeRegistry
    {
        private readonly Dictionary<string, TypeSymbol> _types = new(StringComparer.OrdinalIgnoreCase);

        public List<TypeSymbol> AllTypes => [.. _types.Values];

        public void Register(string name, QxType type)
        {
            _types[name] = new TypeSymbol(name, type);
        }

        public bool Contains(string name)
        {
            return _types.ContainsKey(name);
        }

        public QxType? Resolve(string name)
        {
            _types.TryGetValue(name, out var type);
            return type?.Type;
        }

        public bool TryResolve(string name, [NotNullWhen(returnValue: true)] out QxType? type)
        {
            if (!_types.TryGetValue(name, out var typeSymbol))
            {
                type = null;
                return false;
            }

            type = typeSymbol.Type;
            return true;
        }
    }
}
