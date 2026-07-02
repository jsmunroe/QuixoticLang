using System.ComponentModel;

namespace Quixotic.Common.Types
{
    [Description("array")]
    public class ArrayType(QxType elementType) : QxType($"{elementType}[]")
    {
        public QxType ElementType { get; } = elementType;

        public override bool IsAssignableFrom(QxType subtype)
        {
            if (subtype is not ArrayType arrayType)
                return false;

            if (!ElementType.IsAssignableFrom(arrayType.ElementType))
                return false;

            return true;
        }
    }
}
