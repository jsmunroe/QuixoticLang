namespace Quixotic.Common.Exceptions.Interpret
{
    public class UntypedVariableDeclarationException() : Exception("Variable declaration is missing a type, and type cannot be inferred from the assigned value.");
}
