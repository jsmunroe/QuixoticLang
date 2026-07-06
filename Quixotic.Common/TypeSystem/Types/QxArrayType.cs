using System.ComponentModel;

namespace Quixotic.Common.TypeSystem.Types
{
    [Description("array")]
    public class QxArrayType(QxType elementType) : QxType($"{elementType}[]")
    {
        public QxType ElementType { get; } = elementType;

        public override bool IsAssignableFrom(QxType subtype)
        {
            if (subtype is not QxArrayType arrayType)
                return false;

            if (!ElementType.IsAssignableFrom(arrayType.ElementType))
                return false;

            return true;
        }
    }
}
