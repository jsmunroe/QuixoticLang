using Quixotic.Common.Statements;
using Quixotic.Common.Tokens;
using System.Text.Json.Serialization;

namespace Quixotic.Common.Diagnostics
{
    public class StatementContext
    {
        public StatementType Type { get; set; }

        [JsonIgnore]
        public QxStatement? Statement { get; set; }

        [JsonIgnore]
        public List<Token> Tokens { get; } = [];

        [JsonIgnore]
        public StatementContext? Parent { get; set; }

        [JsonIgnore]
        public List<StatementContext> Children { get; } = [];

        public List<ActivityContext> Activities { get; } = [];

        [JsonIgnore]
        public Span Span => Tokens.GetTotalSpan();
    }
}
