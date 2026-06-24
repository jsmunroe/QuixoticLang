
namespace Quixotic.Common.Exceptions.Interpret
{
    public class UnexpectedTypeException : InterpreterException
    {
        public UnexpectedTypeException(object expected, object actual)
            : base($"{expected} type was expected, but {actual} type was received.")
        {

        }
    }
}
