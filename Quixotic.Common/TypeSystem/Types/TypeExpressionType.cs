namespace Quixotic.Common.TypeSystem.Types
{
    /// <summary>
    /// Represents a type name expression; something like "typeof"... sort of.
    /// </summary>
    public class TypeExpressionType() : QxType("TypeExpression")
    {
        public Instance Construct(DefinedType type)
        {
            return new Instance(this)
            {
                ["type"] = type
            };
        }

        public static DefinedType GetType(Instance instance)
        {
            return instance.Type is not TypeExpressionType typeExpressionType
                ? throw new InvalidOperationException($"Instance is not of type \"TypeExpression\"")
                : (DefinedType)instance["type"]!;
        }
    }
}
