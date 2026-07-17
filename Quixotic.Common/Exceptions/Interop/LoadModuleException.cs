namespace Quixotic.Common.Exceptions.Interop
{
    public class LoadModuleException : InteropException
    {
        public LoadModuleException(string assemblyFile, Exception inner) : base(GetMessage(assemblyFile), inner)
        { }

        public LoadModuleException(string assemblyFile) : base(GetMessage(assemblyFile))
        { }

        private static string GetMessage(string assemblyFile) => $"Could not load module from '{assemblyFile}'.";
    }
}
