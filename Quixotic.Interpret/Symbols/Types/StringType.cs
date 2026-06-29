using Quixotic.Interpret.Symbols.Values;
using System.ComponentModel;

namespace Quixotic.Interpret.Symbols.Types
{
    [Description("string")]
    public class StringType : QxValueType
    {
        public static StringType Instance { get; } = new();

        protected StringType()
            : base("string", typeof(StringValue))
        { }

        public override bool IsTruthy(Value value)
        {
            if (value is not StringValue stringValue)
                return false;

            return stringValue.Value != string.Empty;
        }
    }
}
