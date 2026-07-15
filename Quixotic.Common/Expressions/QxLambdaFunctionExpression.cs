namespace Quixotic.Common.Expressions
{
    public class QxLambdaFunctionExpression : QxFunctionExpression
    {
        public QxLambdaFunctionExpression(string returnType) : base(returnType)
        { }

        public QxLambdaFunctionExpression(QxFunctionExpression functionExpression) : base(functionExpression)
        { }
    }
}
