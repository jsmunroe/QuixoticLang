namespace Quixotic.Common.Exceptions.Interpret
{
    public class ImmutableSetException() : RuntimeException($"The type set is immutable and cannot be added to.");
}
