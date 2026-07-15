using Quixotic.Common.TypeSystem;
using Quixotic.Common.TypeSystem.Types;

namespace Quixotic.Common.Symbols
{
    public class SignatureSymbol(Signature signature, QxType returnType) : Symbol(signature.Name)
    {
        public SignatureSymbol(string name, QxType returnType, params QxType[] parameters)
            : this(new Signature(name, parameters), returnType)
        { }

        public SignatureSymbol(SignatureSymbol other)
            : this(other.Signature, other.ReturnType)
        { }

        public Signature Signature { get; } = signature;

        public QxType ReturnType { get; } = returnType;

        public List<QxType> ParameterTypes { get; init; } = [.. signature.Parameters];

        public SignatureSymbol Substitute(params QxType[] arguments)
        {
            GenericBindings bindings = new();

            foreach (var (parameter, argument) in ParameterTypes.Zip(arguments))
            {
                if (!parameter.Match(argument, bindings))
                    throw new Exception("Type inference failed");
            }

            return new SignatureSymbol(Name, ReturnType.Substitute(bindings), [.. ParameterTypes.Substitute(bindings)]);
        }

        public override string ToString()
        {
            return $"{Name}({string.Join(", ", ParameterTypes)})";
        }
    }
}
