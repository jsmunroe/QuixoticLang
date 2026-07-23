using Quixotic.Common.TypeSystem;
using Quixotic.Common.TypeSystem.Types;

namespace Quixotic.Common.Symbols
{
    public class SignatureSymbol(Signature signature, QxType returnType, CallType callType) : Symbol(signature.Name)
    {
        public SignatureSymbol(string name, QxType returnType, QxType[] parameters, CallType callType)
            : this(new Signature(name, parameters), returnType, callType)
        { }

        public SignatureSymbol(SignatureSymbol other)
            : this(other.Signature, other.ReturnType, other.CallType)
        { }

        public Signature Signature { get; } = signature;

        public QxType ReturnType { get; } = returnType;

        public CallType CallType { get; } = callType;

        public List<QxType> ParameterTypes { get; init; } = [.. signature.Parameters];

        public virtual bool Match(QxType[] parameters, GenericBindings bindings)
        {
            if (parameters.Length != ParameterTypes.Count)
                return false;

            foreach (var (thisParam, otherParam) in ParameterTypes.Zip(parameters))
            {
                if (!thisParam.Match(otherParam, bindings))
                    return false;
            }

            return true;
        }

        public virtual SignatureSymbol Substitute(GenericBindings bindings)
        {
            var signature = Signature.Substitute(bindings);

            var returnType = ReturnType.Substitute(bindings);

            return new SignatureSymbol(signature, returnType, CallType);
        }

        public SignatureSymbol WithName(string name)
        {
            return new(name, ReturnType, [.. ParameterTypes], CallType);
        }

        public override string ToString()
        {
            return $"{Name}({string.Join(", ", ParameterTypes)})";
        }
    }
}
