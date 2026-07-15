using Quixotic.Common.TypeSystem;
using Quixotic.Common.TypeSystem.Symbols;
using Quixotic.Common.TypeSystem.Types;
using System.Diagnostics.CodeAnalysis;

namespace Quixotic.Common.Environment
{
    public class TypeRegistry
    {
        private readonly Dictionary<TypeName, TypeSymbol> _types = []; // case-insensitivity is handles by TypeName class

        public List<TypeSymbol> AllTypes => [.. _types.Values];

        public void Add(TypeRegistry other)
        {
            foreach (var type in other.AllTypes)
                _types[type.Name] = new(type);
        }

        public TypeRegistry Capture(ClosureCapture closureCapture)
        {
            var registry = new TypeRegistry();

            foreach (var type in AllTypes.Where(closureCapture.IsCaptured))
                registry.Register(type);

            return registry;
        }

        public void Register(string name, QxType type)
        {
            _types[name] = new TypeSymbol(name, type);
        }

        private void Register(TypeSymbol symbol)
        {
            _types[symbol.Name] = symbol;
        }

        public bool Contains(string name)
        {
            return _types.ContainsKey(name);
        }

        public QxType? Resolve(string name)
        {
            return TryResolve(name, out var type) ? type : null;
        }

        public bool TryResolve(string name, [NotNullWhen(returnValue: true)] out QxType? type)
        {
            type = null;

            if (_types.TryGetValue(name, out var typeSymbol))
            {
                type = typeSymbol.Type;

                if (!type.HasGenerics)
                    return true;
            }

            var match = _types.FirstOrDefault(kvp => kvp.Key.IsMatch(name));

            if (match.Key is not null)
            {
                var typeName = match.Key;

                var bindings = typeName.GetGenericBindings(name, this);
                type = match.Value.Type.Substitute(bindings);

                return true;
            }

            return false;
        }
    }
}
