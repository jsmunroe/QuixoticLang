using Quixotic.Analysis.Contracts;
using Quixotic.Common.Contracts;
using Quixotic.Common.Environment;
using Quixotic.Common.Exceptions.Interpret;
using Quixotic.Common.Namespaces;
using Quixotic.Common.Symbols;
using Quixotic.Common.Syntax.Casing;
using Quixotic.Common.Types;
using Quixotic.Common.TypeSystem.Symbols;
using Quixotic.Common.TypeSystem.Types;
using System.Diagnostics.CodeAnalysis;

namespace Quixotic.Analysis.Environment
{
    public class SymbolTable(SymbolTable? parent = null)
    {
        private IdentifierRegistry IdentifierRegistry { get; init; } = new();

        private SignatureRegistry SignatureRegistry { get; init; } = new();

        private TypeRegistry TypeRegistry { get; init; } = new();

        public SymbolTable? Parent { get; } = parent;

        public IEnumerable<SignatureSymbol> Signatures => SignatureRegistry.AllSignatures;

        public IEnumerable<IdentifierSymbol> Identifiers => IdentifierRegistry.AllIdentifiers;

        public IEnumerable<TypeSymbol> Types => TypeRegistry.AllTypes;

        public void Add(ISignatureProvider signatureProvider)
        {
            signatureProvider.Register(SignatureRegistry);
        }

        public void Add(IFunctionProvider functionProvider)
        {
            functionProvider.Register(SignatureRegistry);
        }

        public void Add(ITypeProvider typeProvider)
        {
            typeProvider.Register(TypeRegistry);
        }

        public void Import(Namespace ns)
        {
            TypeRegistry.Import(ns);
        }

        public SymbolTable Capture(ClosureCapture closureCapture)
        {
            if (closureCapture.CaptureAll)
                return this;

            var newParent = Parent?.Capture(closureCapture);

            if (newParent == null)
                return this; // Keep all global scope

            return new SymbolTable(newParent)
            {
                IdentifierRegistry = IdentifierRegistry.Capture(closureCapture),
                SignatureRegistry = SignatureRegistry.Capture(closureCapture),
                TypeRegistry = TypeRegistry.Capture(closureCapture),
            };
        }

        public bool TryDefineVariable(string name, QxType type, QxType? valueType, [NotNullWhen(returnValue: true)] out IdentifierSymbol? identifierSymbol)
        {
            identifierSymbol = null;
            if (IsVariableDeclared(name))
                return false;

            identifierSymbol = IdentifierRegistry.Register(name, type, valueType);
            return true;
        }

        public bool TryDefineVariable(string name, QxType type, QxType? valueType)
        {
            return TryDefineVariable(name, type, valueType, out _);
        }

        public bool TryAssignVariable(string name, QxType type)
        {
            if (!TryGetVariable(name, out var variableSymbol))
                return false;

            if (!variableSymbol.TryAssign(type))
                return false;

            return true;
        }

        public bool IsSignatureDeclared(string name, params QxType[] arguments)
        {
            return SignatureRegistry.Contains(name, arguments) || Parent?.IsSignatureDeclared(name, arguments) == true;
        }

        public bool IsVariableDeclared(string name)
        {
            return IdentifierRegistry.Contains(name) || Parent?.IsVariableDeclared(name) == true;
        }

        public bool IsTypeDeclared(string name)
        {
            return TypeRegistry.Contains(name) || Parent?.IsTypeDeclared(name) == true;
        }

        public bool TryGetVariable(string name, [NotNullWhen(returnValue: true)] out IdentifierSymbol? identifierSymbol)
        {
            if (IdentifierRegistry.TryResolve(name, out var localSymbol))
            {
                identifierSymbol = localSymbol;
                return true;
            }

            if (Parent?.TryGetVariable(name, out var parentSymbol) == true)
            {
                identifierSymbol = parentSymbol;
                return true;
            }

            identifierSymbol = default;
            return false;
        }

        public IdentifierSymbol? GetVariable(string name)
        {
            return TryGetVariable(name, out var identifierSymbol) ? identifierSymbol : null;
        }

        public bool TryDefineSignature(string name, QxType returnType, QxType[] parameters, CallType callType, [NotNullWhen(returnValue: true)] out SignatureSymbol? signatureSymbol)
        {
            signatureSymbol = null;

            if (IsSignatureDeclared(name, parameters))
                return false;

            signatureSymbol = SignatureRegistry.Register(name, returnType, parameters, callType);
            return true;
        }

        public bool TryDefineSignature(string name, SignatureSymbol signatureSymbol)
        {
            if (IsSignatureDeclared(name, [.. signatureSymbol.ParameterTypes]))
                return false;

            SignatureRegistry.Register(signatureSymbol.WithName(name));
            return true;
        }

        public bool TryDefineConstructorSignature(QxType type, QxType[] parameters)
        {
            return TryDefineConstructorSignature(type, parameters, out _);
        }

        public bool TryDefineConstructorSignature(QxType type, QxType[] parameters, [NotNullWhen(returnValue: true)] out SignatureSymbol? signatureSymbol)
        {
            signatureSymbol = null;

            var name = "::constructor";

            if (IsSignatureDeclared(name, parameters))
                return false;

            signatureSymbol = SignatureRegistry.Register("::constructor", type, parameters, CallType.ConstructorCall);
            return true;
        }

        public bool TryDefineSignature(string name, QxType returnType, QxType[] parameters, CallType functionCallType)
        {
            return TryDefineSignature(name, returnType, parameters, functionCallType, out _);
        }

        public bool TryGetSignatureByName(string name, [NotNullWhen(returnValue: true)] out SignatureSymbol? signatureSymbol)
        {
            signatureSymbol = null;

            var signatures = SignatureRegistry.AllSignatures.Where(s => CaseRule.Current.Equals(s.Name, name)).ToArray();

            if (signatures.Length == 1)
            {
                signatureSymbol = signatures[0];
                return true;
            }

            return false; // There were no signatures or more than one.
        }

        public bool TryGetSignature(string name, QxType[] arguments, [NotNullWhen(returnValue: true)] out SignatureSymbol? signatureSymbol)
        {
            signatureSymbol = null;

            var signature = new Signature(name, arguments);

            if (SignatureRegistry.TryResolve(signature, out signatureSymbol))
                return true;

            return Parent?.TryGetSignature(name, arguments, out signatureSymbol) == true;
        }

        public SignatureSymbol? GetSignature(string name, params QxType[] arguments)
        {
            return TryGetSignature(name, arguments, out var signatureSymbol) ? signatureSymbol : null;
        }

        public SignatureSymbol? GetSignatureFromType(QxType type, string name, params QxType[] arguments)
        {
            if (!type.TryResolveMethod(name, arguments, out var function))
                return null;

            var signature = new Signature(name, [.. function.Parameters.GetTypes()]);

            return new SignatureSymbol(signature, function.ReturnType, function.CallType);
        }

        public bool TryDefineType(string name, QxType type)
        {
            if (IsTypeDeclared(name))
                return false;

            TypeRegistry.Register(name, type);
            return true;
        }

        public QxType GetType(string name)
        {
            if (TypeRegistry.TryResolve(name, out var type))
                return type;

            if (Parent is not null)
                return Parent.GetType(name);

            throw new UndefinedTypeException(name);
        }

        public bool TryGetType(string name, [NotNullWhen(returnValue: true)] out QxType? type)
        {
            if (TypeRegistry.TryResolve(name, out type))
                return true;

            return Parent?.TryGetType(name, out type) == true;
        }

        public bool IsDefined(string name)
        {
            return TryGetVariable(name, out _);
        }

    }
}
