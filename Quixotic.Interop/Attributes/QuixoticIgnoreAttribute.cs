namespace Quixotic.Interop.Attributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Method)]
    public class QuixoticIgnoreAttribute : Attribute
    { }
}
