using Quixotic.Common.Extensions;
using System.Text.Json.Serialization;

namespace Quixotic.Common.Tokens
{

    public class Token : TokenHead
    {
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWriting)]
        public required Span Span { get; init; }

        public override string ToString() => $"{Type.ToText().ToLower()} '{Value}'";
    }
}
