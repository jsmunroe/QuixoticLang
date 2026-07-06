using System.ComponentModel;

namespace Quixotic.Common.TypeSystem.Types
{
    [Description("string")]
    public class StringType : QxValueType
    {
        public static StringType Instance { get; } = new();

        protected StringType()
            : base("string", typeof(string))
        { }
    }
}
