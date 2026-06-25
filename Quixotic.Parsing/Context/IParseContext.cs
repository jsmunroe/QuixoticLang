using Quixotic.Common.Expressions;
using Quixotic.Common.Statements;
using Quixotic.Common.Tokens;

namespace Quixotic.Parsing.Context
{
    public interface IParseContext
    {
        ExpressionContext? CurrentExpression { get; }
        StatementContext? CurrentStatement { get; }

        void ConsumeToken(Token token);

        void BeginStatement();
        void AttachStatement(QxStatement statement);
        void EndStatement();
        void BeginExpression();
        void AttachExpression(QxExpression expression);
        void EndExpression();
    }
}