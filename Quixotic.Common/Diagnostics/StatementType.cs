namespace Quixotic.Common.Diagnostics
{
    public enum StatementType
    {
        Unknown = 0,
        Print,
        Identifier,
        Assignment,
        If,
        Do,
        For,
        Break,
        Continue,
        VariableDeclaration,
        FunctionDeclaration,
        FunctionCall,
        Return,
        StandaloneExpression,
    }
}
