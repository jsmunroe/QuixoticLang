using Quixotic.Analysis.BuiltIn;
using Quixotic.Common.Exceptions.Interpret;
using Quixotic.Common.Symbols;
using Quixotic.Common.Types;
using System.Diagnostics.CodeAnalysis;

namespace Quixotic.Analysis.Environment
{
    public class SymbolTable(SymbolTable? parent = null)
    {
        private readonly Dictionary<string, VariableTypeSymbol> _variables = [];

        private readonly SignatureRegistry _signatureRegistry = new();

        public void Add(ISignatureProvider signatureProvider)
        {
            signatureProvider.Register(_signatureRegistry);
        }

        public void DefineVariable(string name, QxType type)
        {
            ExpectUndefined(name);

            _variables[name] = new VariableTypeSymbol(type);
        }

        public bool IsSymbolDeclared(string name)
        {
            return _variables.ContainsKey(name);
        }

        public bool IsFunctionDeclared(string name, params QxType[] arguments)
        {
            return _signatureRegistry.Contains(name, arguments);
        }

        public bool IsVariableDeclared(string name)
        {
            return _variables.ContainsKey(name);
        }

        private bool TryGetSymbol(string name, [NotNullWhen(returnValue: true)] out Symbol? symbol)
        {
            if (_variables.TryGetValue(name, out var localSymbol))
            {
                symbol = localSymbol;
                return true;
            }

            if (parent?.TryGetSymbol(name, out var parentSymbol) == true)
            {
                symbol = parentSymbol;
                return true;
            }

            symbol = default;
            return false;
        }

        public void AssignVariable(string name, QxType instance)
        {
            if (!TryGetSymbol(name, out var symbol) || symbol is not VariableTypeSymbol variableSymbol)
                throw new UndefinedSymbolException(name);
        }

        public QxType GetInstance(string name)
        {
            if (!TryGetSymbol(name, out var symbol) || symbol is not VariableTypeSymbol variableSymbol)
                throw new UndefinedSymbolException(name);

            return variableSymbol.Type;
        }

        public void DefineSignature(string name, QxType returnType, params QxType[] parameters)
        {
            ExpectUndefined(name);

            _signatureRegistry.Register(name, returnType, parameters);
        }

        public FunctionSignatureSymbol GetSignature(string name, params QxType[] arguments)
        {
            if (_signatureRegistry.TryResolve(name, arguments, out var functionSymbol))
                return functionSymbol;

            throw new UndefinedFunctionException(name);
        }

        private void ExpectUndefined(string name)
        {
            if (TryGetSymbol(name, out var symbol))
            {
                if (symbol is VariableTypeSymbol)
                    throw new VariableAlreadyDefinedException(name);

                if (symbol is FunctionSignatureSymbol)
                    throw new FunctionAlreadyDefinedException(name);

                throw new SymbolAlreadyDefinedException(name);
            }

            parent?.ExpectUndefined(name);
        }

    }
}
