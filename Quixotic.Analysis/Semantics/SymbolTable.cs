
using Quixotic.Common.Exceptions.Interpret;
using Quixotic.Common.Symbols;
using Quixotic.Common.Types;
using System.Diagnostics.CodeAnalysis;

namespace Quixotic.Analysis.Semantics
{
    public class SymbolTable(SymbolTable? parent = null)
    {
        private readonly Dictionary<string, Symbol> _values = [];

        public void DefineVariable(string name, QxType type)
        {
            ExpectUndefined(name);

            _values[name] = new VariableTypeSymbol(type);
        }

        public bool IsSymbolDeclared(string name)
        {
            return _values.ContainsKey(name);
        }

        public bool IsFunctionDeclared(string name)
        {
            return _values.TryGetValue(name, out var symbol) && symbol is FunctionTypeSymbol;
        }

        public bool IsVariableDeclared(string name)
        {
            return _values.TryGetValue(name, out var symbol) && symbol is VariableTypeSymbol;
        }

        private bool TryGetSymbol(string name, [NotNullWhen(returnValue: true)] out Symbol? symbol)
        {
            if (_values.TryGetValue(name, out var localSymbol))
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

        public QxType GetVariableType(string name)
        {
            if (!TryGetSymbol(name, out var symbol) || symbol is not VariableTypeSymbol variableSymbol)
                throw new UndefinedSymbolException(name);

            return variableSymbol.Type;
        }

        public void DefineFunction(string name, QxType returnValue, params QxType[] parameterValues)
        {
            ExpectUndefined(name);

            _values[name] = new FunctionTypeSymbol(returnValue) { ParameterTypes = [.. parameterValues] };
        }

        public FunctionTypeSymbol GetFunction(string name)
        {
            if (!TryGetSymbol(name, out var symbol) || symbol is not FunctionTypeSymbol functionSymbol)
                throw new UndefinedFunctionException(name);

            return functionSymbol;
        }

        private void ExpectUndefined(string name)
        {
            if (TryGetSymbol(name, out var symbol))
            {
                if (symbol is VariableTypeSymbol)
                    throw new VariableAlreadyDefinedException(name);

                if (symbol is FunctionTypeSymbol)
                    throw new FunctionAlreadyDefinedException(name);

                throw new SymbolAlreadyDefinedException(name);
            }

            parent?.ExpectUndefined(name);
        }

    }
}
