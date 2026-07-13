namespace Quixotic.Common.Exceptions.Interpret
{
    public class InterpreterException : Exception
    {
        public InterpreterException(string message, Exception innerException) : base(message, innerException)
        { }

        public InterpreterException(string message) : base(message)
        { }
    }
}
