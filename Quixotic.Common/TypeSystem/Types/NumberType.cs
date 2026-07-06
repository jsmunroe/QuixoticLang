using System.ComponentModel;

namespace Quixotic.Common.TypeSystem.Types
{
    [Description("number")]
    public class NumberType : QxValueType
    {
        public static NumberType Instance { get; } = new();

        protected NumberType()
            : base("number", typeof(double))
        { }
    }
}
