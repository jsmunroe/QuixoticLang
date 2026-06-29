using System.ComponentModel;

namespace Quixotic.Common.Tokens
{
    public enum TokenType
    {
        StringLiteral,
        NumberLiteral,
        True,
        False,
        Identifier,
        Print,
        Let,
        If,
        Then,
        Else,
        End,
        Do,
        While,
        Until,
        Loop,
        Continue,
        Break,
        EqualTo,
        NotEqualTo,
        GreaterThan,
        GreaterThanOrEqualTo,
        LessThan,
        LessThanOrEqualTo,
        Assignment,

        [Description("Open Parenthesis")]
        OpenParen,

        [Description("Close Parenthesis")]
        CloseParen,

        Subtract,
        Plus,
        Multiply,
        Divide,
        Comma,
        Or,
        And,
        Not,
        Function,
        Return,
        NewLine,
        Eof,
        For,
        To,
        Next,
        Step,
        Comment,
        Type,
    }
}
