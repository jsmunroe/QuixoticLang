using Quixotic.Common.Contracts;
using Quixotic.Common.Exceptions.Interpret;
using Quixotic.Common.Namespaces;
using Quixotic.Common.Symbols;
using Quixotic.Common.Symbols.Functions;
using Quixotic.Common.Syntax.Casing;
using Quixotic.Common.Tokens;
using Quixotic.Common.Types;
using Quixotic.Common.TypeSystem;
using Quixotic.Common.TypeSystem.Symbols;
using Quixotic.Common.TypeSystem.Types;
using System.Diagnostics.CodeAnalysis;

namespace Quixotic.Common.Environment
{
    // I changed this to scope because C# already has a static Environment object
    // under the System namespace, and it was colliding with this class.
    public class Scope(Scope? parent)
    {
        private VariableRegistry VeraiableRegistry { get; init; } = new();

        private FunctionRegistry FunctionRegistry { get; init; } = new();

        private TypeRegistry TypeRegistry { get; init; } = new();

        public List<FunctionSymbol> Functions => FunctionRegistry.AllFunctions;

        public List<VariableSymbol> Variables => VeraiableRegistry.AllVariables;

        public List<TypeSymbol> Types => TypeRegistry.AllTypes;

        public void Add(IFunctionProvider functionProvider)
        {
            functionProvider.Register(FunctionRegistry);
        }

        public void Add(ITypeProvider typeProvider)
        {
            typeProvider.Register(TypeRegistry);
        }

        public void Add(ScopeState scopeState)
        {
            FunctionRegistry.Add(scopeState.Functions);
            TypeRegistry.Add(scopeState.Types);
            VeraiableRegistry.Add(scopeState.Variables);
        }
        public void Import(Namespace ns)
        {
            TypeRegistry.Import(ns);
        }

        public Scope Capture(ClosureCapture closureCapture)
        {
            if (closureCapture.CaptureAll)
                return this;

            var newParent = parent?.Capture(closureCapture);

            if (newParent == null)
                return this; // Keep all global scope

            return new Scope(newParent)
            {
                VeraiableRegistry = VeraiableRegistry.Capture(closureCapture),
                FunctionRegistry = FunctionRegistry.Capture(closureCapture),
                TypeRegistry = TypeRegistry.Capture(closureCapture),
            };
        }

        public void DefineVariable(string name, Instance instance)
        {
            if (IsVariableDeclared(name))
                throw new VariableAlreadyDefinedException(name);


            VeraiableRegistry.Register(name, instance);
        }

        public void DefineVariable(string name, QxType type)
        {
            if (IsVariableDeclared(name))
                throw new VariableAlreadyDefinedException(name);

            VeraiableRegistry.Register(name, type);
        }

        public bool IsFunctionDeclared(string name, params QxType[] parameters)
        {
            return FunctionRegistry.Contains(name, parameters);
        }

        public bool IsVariableDeclared(string name)
        {
            return VeraiableRegistry.Contains(name);
        }

        public bool IsTypeDeclared(string name)
        {
            return TypeRegistry.Contains(name);
        }

        private bool TryGetSymbol(string name, [NotNullWhen(returnValue: true)] out Symbol? symbol)
        {
            if (VeraiableRegistry.TryResolve(name, out var localSymbol))
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

            if (FunctionRegistry.Contains(signature))
                throw new FunctionAlreadyDefinedException(signature);

            FunctionRegistry.Register(name, function);
        }

        public void DefineConstructor(Constructor constructor)
        {
            DefineFunction("::constructor", constructor);
        }

        public List<Function> GetFunctionsByName(string name)
        {
            return [.. FunctionRegistry.AllFunctions.Where(s => CaseRule.Current.Equals(s.Name, name)).Select(s => s.Function)];
        }

        public Function GetFunction(string name, params QxType[] arguments)
        {
            return TryGetFunction(name, arguments, out var function) ? function : throw new UndefinedFunctionException(name, Span.Empty); // TODO: Figure out actual Span
        }

        public bool TryGetFunction(string name, QxType[] arguments, [NotNullWhen(returnValue: true)] out Function? function)
        {
            function = null;

            if (FunctionRegistry.TryResolve(name, arguments, out var functionSymbol))
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

            TypeRegistry.Register(name, type);
        }

        public QxType GetType(string name)
        {
            if (TypeRegistry.TryResolve(name, out var type))
                return type;

            if (parent is not null)
                return parent.GetType(name);

            throw new UndefinedTypeException(name);
        }

        public bool TryGetType(string name, [NotNullWhen(returnValue: true)] out QxType? type)
        {
            if (TypeRegistry.TryResolve(name, out type))
                return true;

            if (parent is null)
                return false;

            return parent.TryGetType(name, out type);
        }
    }
}
