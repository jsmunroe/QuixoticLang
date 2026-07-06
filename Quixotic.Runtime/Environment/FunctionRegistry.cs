using Quixotic.Common.Expressions;
using Quixotic.Common.Statements;
using Quixotic.Common.Symbols;
using Quixotic.Common.Types;
using Quixotic.Interpret.Expressions;
using Quixotic.Runtime.Symbols;
using System.Diagnostics.CodeAnalysis;

namespace Quixotic.Runtime.Environment
{
    public class FunctionRegistry
    {
        private readonly Dictionary<Signature, FunctionSymbol> _functions = [];

        public void Register(string name, Function function)
        {
            var parameterTypes = function.Parameters.GetTypes();

            var signature = new Signature(name, [.. parameterTypes]);

            var functionSymbol = new FunctionSymbol(name, function);

            _functions[signature] = functionSymbol;
        }

        public void Register(string name, Delegate implementation, QxType returnType, params Parameter[] parameters)
        {
            var parameterTypes = parameters.Select(p => p.Type);

            var parameterExpressions = parameters.Select(p => new QxIdentifierExpression(p.Name));

            var signature = new Signature(name, [.. parameterTypes]);
            var externalCall = new QxExternalCallExpression(implementation) { Arguments = [.. parameterExpressions] };
            var returnStatement = new QxReturnStatement(externalCall);

            var function = new Function([returnStatement], returnType) { Parameters = [.. parameters] };
            var functionSymbol = new FunctionSymbol(name, function);

            _functions[signature] = functionSymbol;
        }

        public bool Contains(string name, params QxType[] arguments)
        {
            return TryResolve(name, arguments, out _);
        }

        public FunctionSymbol? Resolve(string name, params QxType[] parameterTypes)
        {
            return TryResolve(name, parameterTypes, out var functionSymbol) ? functionSymbol : null;
        }

        public bool TryResolve(string name, QxType[] parameterTypes, [NotNullWhen(returnValue: true)] out FunctionSymbol? functionSymbol)
        {
            functionSymbol = null;

            var signature = new Signature(name, [.. parameterTypes]);

            if (_functions.TryGetValue(signature, out functionSymbol))
                return true;

            foreach (var (s, f) in _functions)
            {
                if (s.IsCompatible(name, parameterTypes))
                {
                    functionSymbol = f;
                    return true;
                }
            }

            return false;
        }
    }
}
