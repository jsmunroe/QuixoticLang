using System.ComponentModel;

namespace Quixotic.Interpret.Symbols.Types
{
    [Description("array")]
    public class ArrayType(QxType elementType) : QxType($"{elementType}[]")
    {
        public QxType ElementType { get; } = elementType;
    }

}
