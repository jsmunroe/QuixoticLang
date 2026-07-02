using Quixotic.Common.Exceptions.Interpret;
using Quixotic.Common.Symbols;
using Quixotic.Common.Types;
using Quixotic.Interpret.Contracts;
using Quixotic.Interpret.Symbols;
using Quixotic.Interpret.Symbols.Instances;
using System.Diagnostics.CodeAnalysis;

namespace Quixotic.Interpret.Environment
{
    // I changed this to scope because C# already has a static Environment object
    // under the System namespace, and it was colliding with this class.
    public class Scope(Scope? parent)
    {
        private readonly Dictionary<string, Symbol> _values = [];

        private readonly FunctionRegistry _functionRegistry = new();

        public void Add(IFunctionProvider functionProvider)
        {
            functionProvider.Register(_functionRegistry);
        }

        public void DefineVariable(string name, Instance instance)
        {
            ExpectUndefined(name);

            _values[name] = new VariableSymbol(instance);
        }

        public void DefineVariable(string name, QxType type)
        {
            ExpectUndefined(name);

            _values[name] = new VariableSymbol(type);
        }

        public bool IsSymbolDeclared(string name)
        {
            return _values.ContainsKey(name);
        }

        public bool IsFunctionDeclared(string name)
        {
            return _values.TryGetValue(name, out var symbol) && symbol is FunctionSymbol;
        }

        public bool IsVariableDeclared(string name)
        {
            return _values.TryGetValue(name, out var symbol) && symbol is VariableSymbol;
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

        public void AssignVariable(string name, Instance instance)
        {
            if (!TryGetSymbol(name, out var symbol) || symbol is not VariableSymbol variableSymbol)
                throw new UndefinedSymbolException(name);

            variableSymbol.Assign(instance); // Throws TypeConversionException if type of value is not type of set identifer.
        }

        public Instance GetInstance(string name)
        {
            if (!TryGetSymbol(name, out var symbol) || symbol is not VariableSymbol variableSymbol)
                throw new UndefinedSymbolException(name);

            return variableSymbol.Instance;
        }

        public void DefineFunction(string name, Function function)
        {
            ExpectUndefined(name);

            _values[name] = new FunctionSymbol(name, function);
            _functionRegistry.Register(name, function);
        }

        public Function GetFunction(string name, params QxType[] arguments)
        {
            if (_functionRegistry.TryResolve(name, arguments, out var functionSymbol))
                return functionSymbol.Function;

            if (parent is not null)
                return parent.GetFunction(name, arguments);

            throw new UndefinedFunctionException(name);
        }

        private void ExpectUndefined(string name)
        {
            if (TryGetSymbol(name, out var symbol))
            {
                if (symbol is VariableSymbol)
                    throw new VariableAlreadyDefinedException(name);

                if (symbol is FunctionSymbol)
                    throw new FunctionAlreadyDefinedException(name);

                throw new SymbolAlreadyDefinedException(name);
            }

            parent?.ExpectUndefined(name);
        }

    }
}
