using Quixotic.Common.Expressions;
using Quixotic.Common.Tokens;
using System.Text.Json.Serialization;

namespace Quixotic.Common.Diagnostics
{
    public class ActivityContext(ActivityType expressionType)
    {
        public ActivityType Type { get; } = expressionType;

        [JsonIgnore]
        public List<Token> Tokens { get; } = [];

        [JsonIgnore]
        public ActivityContext? Parent { get; set; }

        [JsonIgnore]
        public List<ActivityContext> Children { get; } = [];

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWriting)]
        public Span Span => Tokens.GetTotalSpan();

        [JsonIgnore]
        public QxExpression? Expression { get; set; }
    }
}
