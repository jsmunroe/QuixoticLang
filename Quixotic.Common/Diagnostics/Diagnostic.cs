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
        public bool IsRootActivity => Activity is not null && Activity.Parent is null;

        public string? LastIdentifier => Statement?.Tokens.FindValues(TokenType.Identifier).LastOrDefault();
        public Token? LastConsumedToken => Statement?.Tokens.LastOrDefault();

        public ActivityContext? RootActivity
        {
            get
            {
                if (Activity is null)
                    return null;

                var activity = Activity;
                while (activity.Parent is not null)
                    activity = activity.Parent;

                return activity;
            }
        }
    }
}
