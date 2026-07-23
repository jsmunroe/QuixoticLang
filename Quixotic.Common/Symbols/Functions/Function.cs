using Quixotic.Common.Environment;
using Quixotic.Common.Exceptions.Interpret;
using Quixotic.Common.Expressions;
using Quixotic.Common.Statements;
using Quixotic.Common.TypeSystem;
using Quixotic.Common.TypeSystem.Symbols;
using Quixotic.Common.TypeSystem.Types;

namespace Quixotic.Common.Symbols.Functions
{
    public class Function(Block body, QxType returnType, CallType callType)
    {
        public Function(Function other)
            : this([.. other.Body], other.ReturnType, other.CallType)
        {
            Parameters = [.. other.Parameters];
            Closure = other.Closure;
        }

        public List<Parameter> Parameters { get; init; } = [];

        public Block Body { get; } = body;

        public QxType ReturnType { get; private set; } = returnType;

        public CallType CallType { get; } = callType;

        public Scope? Closure { get; init; }

        public virtual Function Substitute(GenericBindings bindings)
        {
            List<Parameter> parameters = [];
            foreach (var parameter in Parameters)
            {
                var type = parameter.Type.Substitute(bindings);
                parameters.Add(new Parameter(parameter.Name, type));
            }

            var returnType = ReturnType.Substitute(bindings);

            return new Function(Body, returnType, CallType)
            {
                Parameters = parameters,
                Closure = Closure,
            };
        }

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

        public static Function FromDelegate(ExternalFunction implementation, QxType returnType, CallType callType, params Parameter[] parameters)
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
