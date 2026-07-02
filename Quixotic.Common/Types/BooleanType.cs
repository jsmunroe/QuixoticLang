using System.ComponentModel;

namespace Quixotic.Common.Types
{
    [Description("boolean")]
    public class BooleanType : QxValueType
    {
        public static BooleanType Instance { get; } = new();

        protected BooleanType()
            : base("boolean", typeof(bool))
        { }
    }
}
