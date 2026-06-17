using Quixotic.Lexography.Tokens;

namespace Quixotic.Parsing.Operations
{
    public record OperationMetadata(int Precedence = OperationMetadata.None, string Value = "", Operator Operator = Operator.None, Associativity Associativity = Associativity.Left)
    {
        public const int None = 0;
        public const int Assign = 1;
        public const int Comparison = 5;
        public const int Logical = 3;
        public const int Sum = 10;
        public const int Difference = 10;
        public const int Product = 20;
        public const int Quotient = 20;

        public static readonly Dictionary<TokenType, OperationMetadata> Operators = new()
        {
            [TokenType.Assignment] = new(Assign, ":=", Operator.Assignment, Associativity.Right),
            [TokenType.Subtract] = new(Difference, "-", Operator.Subtract, Associativity.Left),
            [TokenType.Plus] = new(Sum, "+", Operator.Add, Associativity.Left),
            [TokenType.Divide] = new(Quotient, "/", Operator.Divide, Associativity.Left),
            [TokenType.Multiply] = new(Product, "*", Operator.Multiply, Associativity.Left),
            [TokenType.EqualTo] = new(Comparison, "=", Operator.EqualTo, Associativity.Left),
            [TokenType.LessThan] = new(Comparison, "<", Operator.LessThan, Associativity.Left),
            [TokenType.GreaterThan] = new(Comparison, ">", Operator.GreaterThan, Associativity.Left),
            [TokenType.And] = new(Logical, "and", Operator.And, Associativity.Left),
            [TokenType.Or] = new(Logical, "or", Operator.Or, Associativity.Left),
        };

        public static OperationMetadata Get(TokenType type)
        {
            if (Operators.TryGetValue(type, out var metadata))
                return metadata;

            return new();
        }

        public static string? GetOperatorValue(Operator @operator)
        {
            var metadata = Operators.Values.FirstOrDefault(m => m.Operator == @operator);

            return metadata?.Value;
        }
    }
}
