using Quixotic.Common.TypeSystem.Symbols;

namespace Quixotic.Common.TypeSystem.Types
{
    public static class QxTypeExtensions
    {
        public static IEnumerable<QxType> Substitute(this IEnumerable<QxType> types, GenericBindings bindings) => types.Select(t => t.Substitute(bindings));

        public static IEnumerable<Parameter> ToParameters(this IEnumerable<QxType> types, string prefix = "arg") => types.Select((t, i) => new Parameter($"{prefix}{i}", t));
    }
}
