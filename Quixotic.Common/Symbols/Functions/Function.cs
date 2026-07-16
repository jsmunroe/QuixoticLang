using Quixotic.Common.Environment;
using Quixotic.Common.Exceptions.Interpret;
using Quixotic.Common.Expressions;
using Quixotic.Common.Statements;
using Quixotic.Common.TypeSystem;
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

        public Scope? Closure { get; init; }

        public virtual List<Argument> BindArguments(string name, Instance[] instances)
        {
            if (Parameters.Count != instances.Length)
                throw new ParameterCountException(name, Parameters.Count, instances.Length);

            List<Argument> arguments = [];

            // Push function parameters into 
            foreach (var (parameter, instance) in Parameters.Zip(instances))
            {
                if (!parameter.Type.IsAssignableFrom(instance.Type))
                    throw new TypeMismatchException(instance.Type, parameter.Type);

                arguments.Add(new Argument(parameter.Name, instance));
            }

            return arguments;
        }

        public static Function FromDelegate(ExternalFunction implementation, QxType returnType, FunctionCallType callType, params Parameter[] parameters)
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
