using Quixotic.Common.Exceptions.Interpret;
using Quixotic.Common.Symbols;
using Quixotic.Common.TypeSystem.Types;

namespace Quixotic.Common.TypeSystem.Symbols
{
    public class VariableSymbol : IdentifierSymbol
    {
        public Instance Instance { get; private set; }

        public VariableSymbol(string name, Instance instance)
            : base(name, instance.Type, instance.Type)
        {
            Instance = instance;
        }

        public VariableSymbol(string name, QxType type)
            : base(name, type, null)
        {
            Instance = NadaType.Value;
        }

        public VariableSymbol(VariableSymbol other)
            : base(other)
        {
            Instance = other.Instance;
        }

        public void Assign(Instance instance)
        {
            if (Type is DeferredType deferredType)
            {
                if (deferredType.ContextDependency == ContextDependency.VariableAssignment)
                    Type = instance.Type;
                else if (deferredType.HasAlternative)
                    Type = deferredType.SelectedAlternative;
            }

            if (Type != QxType.Nada.Type && !instance.Type.IsAssignableFrom(Type))
                throw new VariableTypeMismatchException(instance.Type.Name, Type.Name);

            Instance = instance;
        }
    }
}
