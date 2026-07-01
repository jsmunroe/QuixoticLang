namespace Quixotic.Common.Exceptions.Interpret
{
    public class OutOfRangeException(string name) : InterpreterException($"Value {name} is out of range.");
}
