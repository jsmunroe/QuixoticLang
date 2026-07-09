namespace Quixotic.Common.Exceptions.Interpret
{
    public class ConstructorOutsideOfTypeException() : RuntimeException("A constructor cannot be defined outside of a type definition.");
}
