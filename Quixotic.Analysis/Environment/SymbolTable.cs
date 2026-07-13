using Quixotic.Analysis.Contracts;
using Quixotic.Common.Contracts;
using Quixotic.Common.Environment;
using Quixotic.Common.Exceptions.Interpret;
using Quixotic.Common.Symbols;
using Quixotic.Common.TypeSystem.Types;
using System.Diagnostics.CodeAnalysis;

namespace Quixotic.Analysis.Environment
{
    public class SymbolTable(SymbolTable? parent = null)
    {
        private readonly Dictionary<string, VariableTypeSymbol> _variables = [];

        private readonly SignatureRegistry _signatureRegistry = new();

        private readonly TypeRegistry _typeRegistry = new();

        public SymbolTable? Parent { get; } = parent;

        public void Add(ISignatureProvider signatureProvider)
        {
            signatureProvider.Register(_signatureRegistry);
        }

        public void Add(ITypeProvider typeProvider)
        {
            typeProvider.Register(_typeRegistry);
        }

        public bool TryDefineVariable(string name, QxType type)
        {
            if (IsDefined(name))
                return false;

            _variables[name] = new VariableTypeSymbol(name, type);
            return true;
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

        public bool IsTypeDeclared(string name)
        {
            return _typeRegistry.Contains(name);
        }

        private bool TryGetSymbol(string name, [NotNullWhen(returnValue: true)] out Symbol? symbol)
        {
            if (_variables.TryGetValue(name, out var localSymbol))
            {
                symbol = localSymbol;
                return true;
            }

            if (Parent?.TryGetSymbol(name, out var parentSymbol) == true)
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

        public QxType? GetInstance(string name)
        {
            if (!TryGetSymbol(name, out var symbol) || symbol is not VariableTypeSymbol variableSymbol)
                return null;

            return variableSymbol.Type;
        }

        public bool TryDefineSignature(string name, QxType returnType, params QxType[] parameters)
        {
            if (IsDefined(name))
                return false;

            _signatureRegistry.Register(name, returnType, parameters);
            return true;
        }

        public SignatureSymbol? GetSignature(string name, params QxType[] arguments)
        {
            var signature = new Signature(name, [.. arguments]);

            if (_signatureRegistry.TryResolve(signature, out var functionSymbol))
                return functionSymbol;

            if (Parent is not null)
                return Parent.GetSignature(name, arguments);

            return null;
        }

        public void DefineType(string name, QxType type)
        {
            _typeRegistry.Register(name, type);
        }

        public QxType GetType(string name)
        {
            if (_typeRegistry.TryResolve(name, out var type))
                return type;

            if (Parent is not null)
                return Parent.GetType(name);

            throw new UndefinedTypeException(name);
        }

        public bool TryGetType(string name, [NotNullWhen(returnValue: true)] out QxType? type)
        {
            if (_typeRegistry.TryResolve(name, out type))
                return true;

            type = Parent?.GetType(name);
            return type is not null;
        }

        public bool IsDefined(string name)
        {
            return TryGetSymbol(name, out _);
        }

    }
}
