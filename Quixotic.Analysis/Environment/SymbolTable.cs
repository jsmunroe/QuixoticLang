using Quixotic.Analysis.Contracts;
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

        public SymbolTable? Parent { get; } = parent;

        public void Add(ISignatureProvider signatureProvider)
        {
            signatureProvider.Register(_signatureRegistry);
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

        public FunctionSignatureSymbol? GetSignature(string name, params QxType[] arguments)
        {
            var signature = new Signature(name, [.. arguments]);

            if (_signatureRegistry.TryResolve(signature, out var functionSymbol))
                return functionSymbol;

            if (Parent is not null)
                return Parent.GetSignature(name, arguments);

            return null;
        }

        public bool IsDefined(string name)
        {
            return TryGetSymbol(name, out _);
        }

    }
}
