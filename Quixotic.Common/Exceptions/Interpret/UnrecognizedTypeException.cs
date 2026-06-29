namespace Quixotic.Common.Exceptions.Interpret
{
    public class UnrecognizedTypeException(object type) : EnvironmentException($"The type '{type}' is not recognized.");
}
