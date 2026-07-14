using Quixotic.Common.Contracts;
using Quixotic.Common.Environment;
using Quixotic.Common.Exceptions.Interpret;
using Quixotic.Common.Symbols;
using Quixotic.Common.Types;
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
        private readonly VariableRegistry _veraiableRegistry = new();

        private readonly FunctionRegistry _functionRegistry = new();

        private readonly TypeRegistry _typeRegistry = new();

        public List<FunctionSymbol> Functions => _functionRegistry.AllFunctions;

        public List<VariableSymbol> Variables => _veraiableRegistry.AllVariables;

        public List<TypeSymbol> Types => _typeRegistry.AllTypes;

        public void Add(IFunctionProvider functionProvider)
        {
            functionProvider.Register(_functionRegistry);
        }

        public void Add(ITypeProvider typeProvider)
        {
            typeProvider.Register(_typeRegistry);
        }

        public void Add(ScopeState scopeState)
        {
            _functionRegistry.Add(scopeState.Functions);
            _typeRegistry.Add(scopeState.Types);
            _veraiableRegistry.Add(scopeState.Variables);
        }

        public void DefineVariable(string name, Instance instance)
        {
            if (IsVariableDeclared(name))
                throw new VariableAlreadyDefinedException(name);


            _veraiableRegistry.Register(name, instance);
        }

        public void DefineVariable(string name, QxType type)
        {
            if (IsVariableDeclared(name))
                throw new VariableAlreadyDefinedException(name);

            _veraiableRegistry.Register(name, type);
        }

        public bool IsFunctionDeclared(string name, params QxType[] parameters)
        {
            return _functionRegistry.Contains(name, parameters);
        }

        public bool IsVariableDeclared(string name)
        {
            return _veraiableRegistry.Contains(name);
        }

        public bool IsTypeDeclared(string name)
        {
            return _typeRegistry.Contains(name);
        }

        private bool TryGetSymbol(string name, [NotNullWhen(returnValue: true)] out Symbol? symbol)
        {
            if (_veraiableRegistry.TryResolve(name, out var localSymbol))
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
            var signature = new Signature(name, [.. function.Parameters.GetTypes()]);

            if (_functionRegistry.Contains(signature))
                throw new FunctionAlreadyDefinedException(signature);

            _functionRegistry.Register(name, function);
        }

        public Function GetFunction(string name, params QxType[] arguments)
        {
            return TryGetFunction(name, arguments, out var function) ? function : throw new UndefinedFunctionException(name);
        }

        public bool TryGetFunction(string name, QxType[] arguments, [NotNullWhen(returnValue: true)] out Function? function)
        {
            function = null;

            if (_functionRegistry.TryResolve(name, arguments, out var functionSymbol))
            {
                function = functionSymbol.Function;
                return true;
            }

            if (parent is not null)
                return parent.TryGetFunction(name, arguments, out function);

            return false;
        }

        public void DefineType(string name, QxType type)
        {
            if (IsTypeDeclared(name))
                throw new TypeAlreadyDefinedException(name);

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

            if (parent is null)
                return false;

            return parent.TryGetType(name, out type);
        }
    }
}
