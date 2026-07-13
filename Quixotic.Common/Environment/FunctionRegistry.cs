using Quixotic.Common.Expressions;
using Quixotic.Common.Statements;
using Quixotic.Common.Symbols;
using Quixotic.Common.Types;
using Quixotic.Common.TypeSystem.Symbols;
using Quixotic.Common.TypeSystem.Types;
using System.Diagnostics.CodeAnalysis;

namespace Quixotic.Common.Environment
{
    public class FunctionRegistry
    {
        private readonly Dictionary<Signature, FunctionSymbol> _functions = [];

        public List<FunctionSymbol> AllFunctions => [.. _functions.Values];

        public void Add(FunctionRegistry other)
        {
            foreach (var function in other.AllFunctions)
                _functions[function.Signature] = new(function);
        }

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

            Register(signature, functionSymbol);
        }

        public void Register(Signature signature, FunctionSymbol function)
        {
            _functions.Add(signature, function);
        }

        public bool Contains(string name, params QxType[] arguments)
        {
            return TryResolve(name, arguments, out _);
        }

        public bool Contains(Signature signature)
        {
            return TryResolve(signature, out _);
        }

        public IEnumerable<FunctionSymbol> Resolve(string name)
        {
            return _functions.Values.Where(f => f.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
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

        public FunctionSymbol? Resolve(Signature signature) => Resolve(signature.Name, [.. signature.Parameters]);

        public bool TryResolve(Signature signature, [NotNullWhen(returnValue: true)] out FunctionSymbol? functionSymbol) => TryResolve(signature.Name, [.. signature.Parameters], out functionSymbol);

    }
}
