using System.ComponentModel;

namespace Quixotic.Interpret.Symbols.Types
{
    [Description("array")]
    public class ArrayType : QxType
    {
        public static ArrayType WithElement(QxType elementType) => new(elementType);

        protected ArrayType(QxType elementType) : base($"{elementType}[]")
        { }


    }
}
