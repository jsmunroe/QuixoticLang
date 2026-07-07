using Quixotic.Common.TypeSystem;
using Quixotic.Common.TypeSystem.Types;
using System.Diagnostics;

namespace Quixotic.Runtime.Values
{
    [DebuggerDisplay("{ToString()}")]
    public class StringValue : Instance
    {
        public StringValue(string value) : base(QxType.String)
        {
            this["value"] = value;
        }

        public string Value
        {
            get => (string)this["value"]!;
            set => this["value"] = value;
        }
    }
}
