using System.ComponentModel;

namespace Quixotic.Common.Types
{
    [Description("array")]
    public class ArrayType(QxType elementType) : QxType($"{elementType}[]")
    {
        public QxType ElementType { get; } = elementType;
    }
}
