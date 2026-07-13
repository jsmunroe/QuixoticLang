using Quixotic.Common.TypeSystem.Types;
using System.Diagnostics.CodeAnalysis;

namespace Quixotic.Common.TypeSystem
{
    public class GenericBindings
    {
        public readonly Dictionary<string, QxType> _bindings = [];

        public bool TryBind(string name, QxType type)
        {
            if (_bindings.TryGetValue(name, out var existing))
                return existing.Equals(type);

            _bindings[name] = type;
            return true;
        }

        public bool TryGet(string name, [NotNullWhen(returnValue: true)] out QxType? type) => _bindings.TryGetValue(name, out type);
    }
}
