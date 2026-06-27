using Quixotic.Common.Diagnostics.Issues;
using Quixotic.Common.Tokens;
using System.Text.Json.Serialization;

namespace Quixotic.Common.Diagnostics
{
    public record Diagnostic(ContextType ContextType, Issue Issue, StatementContext? Statement, ActivityContext? Activity)
    {
        public StatementType StatementType => Statement?.Type ?? StatementType.Unknown;

        public ActivityType ActivityType => Activity?.ActivityType ?? ActivityType.None;

        [JsonIgnore]
        public Span Span => Statement?.Span ?? LastConsumedToken?.Span ?? Span.Empty;

        public bool IsEndOfLine => LastConsumedToken?.Type == TokenType.NewLine || LastConsumedToken?.Type == TokenType.Eof;

        public Token? LastConsumedToken => Statement?.Tokens.LastOrDefault();
    }
}
