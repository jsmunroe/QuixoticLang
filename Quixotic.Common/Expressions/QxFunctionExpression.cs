using Quixotic.Common.Environment;
using Quixotic.Common.Statements;

namespace Quixotic.Common.Expressions
{
    public class QxFunctionExpression : QxExpression
    {
        public QxFunctionExpression(string returnType)
        {
            ReturnType = returnType;
        }

        public QxFunctionExpression(QxFunctionExpression other)
        {
            ReturnType = other.ReturnType;
            Parameters = other.Parameters;
            WithClosure = other.WithClosure;
            Body = other.Body;
        }

        public string ReturnType { get; }

        public List<QxParameter> Parameters { get; init; } = [];

        public ClosureCapture? WithClosure { get; init; }

        public Block Body { get; init; } = [];
    }
}
