namespace Quixotic.Common.Exceptions.Interop
{
    public class AssemblyNotFoundException(object @namespace) : InteropException($"Assembly '{@namespace}' could not be located.");
}
