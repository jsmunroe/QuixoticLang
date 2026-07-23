using Quixotic.Common.TypeSystem.Types;

namespace Quixotic.Common.Symbols
{
    public class IdentifierSymbol(string name, QxType type, QxType? valueType) : Symbol(name)
    {
        public IdentifierSymbol(IdentifierSymbol identifierSymbol)
            : this(identifierSymbol.Name, identifierSymbol.Type, identifierSymbol.ValueType)
        { }

        public QxType Type { get; set; } = type;

        public QxType? ValueType { get; private set; } = valueType;

        public bool TryAssign(QxType valueType)
        {
            if (Type is DeferredType deferredType)
            {
                if (deferredType.ContextDependency == ContextDependency.VariableAssignment)
                    Type = valueType;
                else if (deferredType.HasAlternative)
                    Type = deferredType.SelectedAlternative;
            }

            if (!Type.IsAssignableFrom(valueType))
                return false;

            ValueType = valueType;
            return true;
        }
    }
}
