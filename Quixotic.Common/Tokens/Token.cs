using System.Text.Json.Serialization;

namespace Quixotic.Common.Tokens
{

    public class Token : TokenHead
    {
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWriting)]
        public required Span Span { get; init; }

        public override string ToString() => $"{Type}('{Value}') ({Span.Start.Line}:{Span.Start.Column})";
    }
}
