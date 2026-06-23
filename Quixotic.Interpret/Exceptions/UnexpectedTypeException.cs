using Quixotic.Interpret.Symbols;

namespace Quixotic.Interpret.Exceptions
{
    public class UnexpectedTypeException : InterpreterException
    {
        public UnexpectedTypeException(QxType expected, QxType actual)
            : base($"{expected} type was expected, but {actual} type was received.")
        {

        }
    }
}
