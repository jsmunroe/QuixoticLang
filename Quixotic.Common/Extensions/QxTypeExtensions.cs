namespace Quixotic.Common.TypeSystem.Types
{
    public static class QxTypeExtensions
    {
        public static IEnumerable<QxType> Substitute(this IEnumerable<QxType> types, GenericBindings bindings) => types.Select(t => t.Substitute(bindings));
    }
}
