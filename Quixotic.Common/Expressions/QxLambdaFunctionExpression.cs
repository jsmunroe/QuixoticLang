using Quixotic.Common.Symbols;

namespace Quixotic.Common.Expressions
{
    public class QxLambdaFunctionExpression : QxFunctionExpression
    {
        public QxLambdaFunctionExpression(string returnType, CallType callType) : base("::inline", returnType, callType, isGlobalOrStatic: false)
        { }

        public QxLambdaFunctionExpression(QxFunctionExpression functionExpression) : base(functionExpression)
        { }
    }
}
