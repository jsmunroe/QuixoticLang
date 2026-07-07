using Quixotic.Common.Exceptions.Interpret;
using Quixotic.Common.Symbols;
using Quixotic.Common.TypeSystem;
using Quixotic.Common.TypeSystem.Types;

namespace Quixotic.Runtime.Symbols
{
    public class VariableSymbol : VariableTypeSymbol
    {
        public Instance Instance { get; private set; }

        public VariableSymbol(string name, Instance instance)
            : base(name, instance.Type)
        {
            Instance = instance;
        }

        public VariableSymbol(string name, QxType type)
            : base(name, type)
        {
            Instance = NadaType.Value;
        }

        public void Assign(Instance instance)
        {
            if (Type != QxType.Nada.Type && !instance.Type.Equals(Type))
                throw new TypeMismatchException(instance.Type.Name, Type.Name);

            Instance = instance;
        }
    }
}
