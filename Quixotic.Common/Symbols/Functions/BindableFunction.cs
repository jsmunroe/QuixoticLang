using Quixotic.Common.Exceptions.Interpret;
using Quixotic.Common.Expressions;
using Quixotic.Common.Statements;
using Quixotic.Common.TypeSystem;
using Quixotic.Common.TypeSystem.Symbols;
using Quixotic.Common.TypeSystem.Types;

namespace Quixotic.Common.Symbols.Functions
{
    public class BindableFunction : Function
    {
        public BindableFunction(QxType bindableType, Block body, QxType returnType, FunctionCallType callType) : base(body, returnType, callType)
        {
            BindableType = bindableType;
        }

        public BindableFunction(QxType bindableType, Function function) : base(function)
        {
            BindableType = bindableType;
        }

        public QxType BindableType { get; }

        public virtual BoundFunction Bind(Instance instance)
        {
            if (!BindableType.IsAssignableFrom(instance.Type))
                throw new FunctionBindTypeMismatchException(BindableType, instance.Type);

            return new BoundFunction(instance, this);
        }

        public override List<Argument> BindArguments(string name, Instance[] instances)
        {
            throw new UnboundFunctionException(this);
        }

        public static BindableFunction FromDelegate(QxType bindableType, Delegate implementation, QxType returnType, FunctionCallType callType, params Parameter[] parameters)
        {
            var parameterTypes = parameters.Select(p => p.Type);

            List<QxExpression> parameterExpressions = [new QxIdentifierExpression("this"), .. parameters.Select(p => new QxIdentifierExpression(p.Name))];

            var externalCall = new QxExternalCallExpression(implementation) { Arguments = [.. parameterExpressions] };
            var returnStatement = new QxReturnStatement(externalCall);

            var function = new BindableFunction(bindableType, [returnStatement], returnType, callType) { Parameters = [.. parameters] };

            return function;
        }

        public static BindableFunction Coerce(QxType bindableType, Function function)
        {
            return function is BindableFunction bindableFunction ? bindableFunction : new BindableFunction(bindableType, function);
        }
    }
}
