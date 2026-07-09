using Quixotic.Common.Contracts;
using Quixotic.Common.Environment;
using Quixotic.Common.Exceptions.Interpret;
using Quixotic.Common.Symbols;
using Quixotic.Common.TypeSystem;
using Quixotic.Common.TypeSystem.Symbols;
using Quixotic.Common.TypeSystem.Types;
using System.Diagnostics.CodeAnalysis;

namespace Quixotic.Runtime.Environment
{
    // I changed this to scope because C# already has a static Environment object
    // under the System namespace, and it was colliding with this class.
    public class Scope(Scope? parent)
    {
        private readonly Dictionary<string, Symbol> _values = new(StringComparer.OrdinalIgnoreCase);

        private readonly FunctionRegistry _functionRegistry = new();

        private readonly TypeRegistry _typeRegistry = new();

        public List<Symbol> Symbols => [.. _values.Values];

        public List<FunctionSymbol> Functions => _functionRegistry.AllFunctions;

        public List<VariableSymbol> Variables => [.. _values.Values.OfType<VariableSymbol>()];

        public List<TypeSymbol> Types => [.. _values.Values.OfType<TypeSymbol>()];

        public void Add(IFunctionProvider functionProvider)
        {
            functionProvider.Register(_functionRegistry);
        }

        public void DefineVariable(string name, Instance instance)
        {
            ExpectUndefined(name);

            _values[name] = new VariableSymbol(name, instance);
        }

        public void DefineVariable(string name, QxType type)
        {
            ExpectUndefined(name);

            _values[name] = new VariableSymbol(name, type);
        }

        public bool IsFunctionDeclared(string name)
        {
            return _values.TryGetValue(name, out var symbol) && symbol is FunctionSymbol;
        }

        public bool IsVariableDeclared(string name)
        {
            return _values.TryGetValue(name, out var symbol) && symbol is VariableSymbol;
        }

        public bool IsTypeDeclared(string name)
        {
            return _values.TryGetValue(name, out var symbol) && symbol is TypeSymbol;
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
            if (TryGetInstance(name, out var instance))
                return instance;

            throw new UndefinedSymbolException(name);
        }

        public bool TryGetInstance(string name, [NotNullWhen(returnValue: true)] out Instance? instance)
        {
            instance = null;

            if (!TryGetSymbol(name, out var symbol) || symbol is not VariableSymbol variableSymbol)
                return false;

            instance = variableSymbol.Instance;
            return true;
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

        public void DefineType(string name, QxType type)
        {
            ExpectUndefined(name);

            _values[name] = new TypeSymbol(name, type);
            _typeRegistry.Register(name, type);
        }

        public QxType GetType(string name)
        {
            if (_typeRegistry.TryResolve(name, out var type))
                return type;

            if (parent is not null)
                return parent.GetType(name);

            throw new UndefinedTypeException(name);
        }

        public bool TryGetType(string name, [NotNullWhen(returnValue: true)] out QxType? type)
        {
            if (_typeRegistry.TryResolve(name, out type))
                return true;

            type = parent?.GetType(name);
            return type is not null;
        }

        public void ExpectUndefined(string name)
        {
            if (TryGetSymbol(name, out var symbol))
            {
                if (symbol is VariableSymbol)
                    throw new VariableAlreadyDefinedException(name);

                if (symbol is FunctionSymbol)
                    throw new FunctionAlreadyDefinedException(name);

                if (symbol is TypeSymbol)
                    throw new TypeAlreadyDefinedException(name);

                throw new SymbolAlreadyDefinedException(name);
            }

            parent?.ExpectUndefined(name);
        }

    }
}
