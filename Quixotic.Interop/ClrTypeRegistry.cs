using Quixotic.Common.Contracts;
using Quixotic.Common.Environment;
using Quixotic.Common.Syntax;
using Quixotic.Common.TypeSystem;
using Quixotic.Common.TypeSystem.Types;
using Quixotic.Interop.TypeSystem.Types;
using System.Diagnostics.CodeAnalysis;

namespace Quixotic.Interop
{
    public class ClrTypeRegistry : ITypeProvider, ITypeRegistry
    {
        private readonly Dictionary<Type, ClrType> _types = []; // Type name handles case sensitivity.

        public void Register(TypeRegistry registry)
        {
            throw new NotImplementedException();
        }

        public void Register(Type type, ClrType clrType)
        {
            _types[type] = clrType;
        }

        public bool Contains(Type type)
        {
            return _types.ContainsKey(type);
        }

        public ClrType? Resolve(Type type)
        {
            return TryResolve(type, out var clrType) ? clrType : null;
        }

        public bool TryResolve(Type type, [NotNullWhen(returnValue: true)] out ClrType? clrType)
        {
            return _types.TryGetValue(type, out clrType);
        }

        public Instance Construct(object clrObject)
        {
            if (TryResolve(clrObject.GetType(), out var clrType))
                return clrType.Construct(clrObject);

            throw new Exception($"Cannot construct object of type ${clrObject.GetType().Name}. A wrapper has not been defined.");
        }

        QxType? ITypeRegistry.Resolve(string name) => ((ITypeRegistry)this).TryResolve(name, out var clrType) ? clrType : null;

        bool ITypeRegistry.TryResolve(string name, [NotNullWhen(returnValue: true)] out QxType? type)
        {
            foreach (var kvp in _types)
            {
                if (CaseRule.Current.Equals(kvp.Key.Name, name))
                {
                    type = kvp.Value;
                    return true;
                }
            }

            type = null;
            return false;
        }
    }
}
