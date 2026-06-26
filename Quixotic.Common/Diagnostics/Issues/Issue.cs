using Quixotic.Common.Tokens;

namespace Quixotic.Common.Diagnostics.Issues
{
    public abstract record Issue
    {
        public static DoLoopDualCondition DoLoopDualCondition() => new();
        public static DoLoopNoCondition DoLoopNoCondition() => new();
        public static ExpectedToken ExpectedToken(TokenType expected) => new(expected);
        public static IncompleteSource IncompleteSource() => new();
        public static InvalidNumber InvalidNumber() => new();
        public static UnexpectedToken UnexpectedToken(Token token) => new(token);


    }
}
