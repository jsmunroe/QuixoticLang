using Quixotic.Common.Expressions;
using Quixotic.Common.Statements;
using Quixotic.Common.TypeSystem.Symbols;
using Quixotic.Common.TypeSystem.Types;

namespace Quixotic.Common.Symbols.Functions
{
    public class Function(Block body, QxType returnType, FunctionCallType callType)
    {
        public Function(Function other)
            : this([.. other.Body], other.ReturnType, other.CallType)
        {
            Parameters = [.. other.Parameters];
        }

        public List<Parameter> Parameters { get; init; } = [];

        public Block Body { get; } = body;

        public QxType ReturnType { get; internal set; } = returnType;

        public FunctionCallType CallType { get; } = callType;

        public virtual Function AddThis(QxType thisType)
        {
            return new(Body, ReturnType, CallType)
            {
                Parameters = [new Parameter("this", thisType), .. Parameters],
            };
        }

        public static Function FromDelegate(Delegate implementation, QxType returnType, FunctionCallType callType, params Parameter[] parameters)
        {
            var parameterTypes = parameters.Select(p => p.Type);

            var parameterExpressions = parameters.Select(p => new QxIdentifierExpression(p.Name));

            var externalCall = new QxExternalCallExpression(implementation) { Arguments = [.. parameterExpressions] };
            var returnStatement = new QxReturnStatement(externalCall);

            var function = new Function([returnStatement], returnType, callType) { Parameters = [.. parameters] };

            return function;
        }
    }
}
