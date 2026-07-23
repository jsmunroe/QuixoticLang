using Quixotic.Common.Contracts;
using Quixotic.Common.Symbols.Functions;
using Quixotic.Common.TypeSystem.Symbols;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace Quixotic.Common.TypeSystem.Types
{
    public class FunctionDefinition : TypeDefinition
    {
        private static readonly Regex _functionTypeName = new(@"^function(\(((?<param>\w+),?)*\))?(:(?<return>\w+))?$");

        public static FunctionDefinition Default { get; } = new FunctionDefinition();

        public override bool HasGenerics => false;

        protected FunctionDefinition() : base("function")
        { }

        public Instance Construct(Function function)
        {
            var type = MakeFunctionType(function.ReturnType, [.. function.Parameters]);

            return type.Construct(function);
        }

        public FunctionType MakeFunctionType(QxType returnType, params Parameter[] parameters)
        {
            return new FunctionType(returnType, parameters);
        }

        public override bool Match(QxType actual, GenericBindings bindings)
        {
            if (actual is FunctionDefinition)
                return true;

            if (actual is not FunctionType)
                return false;

            return true;
        }

        public override bool IsAssignableFrom(QxType other)
        {
            if (other is FunctionDefinition || other is FunctionType)
                return true;

            return base.IsAssignableFrom(other);
        }

        public bool TryParseTypeName(string typeName, ITypeRegistry typeRegistry, [NotNullWhen(returnValue: true)] out FunctionType? functionType)
        {
            functionType = null;

            QxType returnType = Void.Type;
            List<QxType> parameterTypes = [];

            var match = _functionTypeName.Match(typeName);

            if (!match.Success)
                return false;

            if (match.Groups["param"].Success)
            {
                foreach (Capture capture in match.Groups["param"].Captures)
                {
                    var parameterTypeName = capture.Value;
                    if (!typeRegistry.TryResolve(parameterTypeName, out var parameterType))
                        return false;

                    parameterTypes.Add(parameterType);
                }
            }

            if (match.Groups["return"].Success)
            {
                var returnTypeName = match.Groups["return"].Value;
                if (!typeRegistry.TryResolve(returnTypeName, out returnType!))
                    return false;
            }

            functionType = MakeFunctionType(returnType, [.. parameterTypes.ToParameters()]);
            return true;
        }
    }
}
