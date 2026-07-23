using Quixotic.Common.Environment;
using Quixotic.Common.Statements;
using Quixotic.Common.Symbols;

namespace Quixotic.Common.Expressions
{
    public class QxFunctionExpression : QxExpression
    {
        public QxFunctionExpression(string name, string returnType, CallType callType, bool isGlobalOrStatic)
        {
            Name = name;
            ReturnType = returnType;
            CallType = callType;
            IsGlobalOrStatic = isGlobalOrStatic;
        }

        public QxFunctionExpression(QxFunctionExpression other)
        {
            Name = other.Name;
            ReturnType = other.ReturnType;
            Parameters = other.Parameters;
            WithClosure = other.WithClosure;
            Body = other.Body;
            CallType = other.CallType;
            IsGlobalOrStatic = other.IsGlobalOrStatic;
        }

        public string Name { get; }
        public string ReturnType { get; set; } // This is writable because it can be changed durring analysis.

        public List<QxParameter> Parameters { get; init; } = [];

        public CallType CallType { get; }
        public bool IsGlobalOrStatic { get; }
        public ClosureCapture? WithClosure { get; init; }

        public Block Body { get; init; } = [];
    }
}
