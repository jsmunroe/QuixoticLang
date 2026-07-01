using Quixotic.Common.Exceptions.Interpret;
using Quixotic.Interpret.Symbols.Instances;
using Quixotic.Interpret.Symbols.Types;

namespace Quixotic.Interpret.Symbols
{
    public class VariableSymbol : Symbol
    {
        public Instance Instance { get; private set; }

        public QxType Type { get; }

        public VariableSymbol(Instance instance)
        {
            Instance = instance;
            Type = instance.Type;
        }

        public VariableSymbol(QxType valueType)
        {
            Instance = Instance.Nada;
            Type = valueType;
        }

        public void Assign(Instance instance)
        {
            if (Type != QxType.Nada && !instance.Type.Equals(Type))
                throw new TypeMismatchException(instance.Type.Name, Type.Name);

            Instance = instance;
        }
    }

    public class FunctionSymbol(Function function) : Symbol
    {
        public Function Function { get; } = function;
    }
}
