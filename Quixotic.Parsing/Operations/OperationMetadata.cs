using Quixotic.Lexography.Tokens;

namespace Quixotic.Parsing.Operations
{
    public record OperationMetadata(int Precedence = OperationMetadata.None, Operator Operator = Operator.None, Associativity Associativity = Associativity.Left)
    {
        public const int None = 0;
        public const int Assign = 1;
        public const int Sum = 10;
        public const int Difference = 15;
        public const int Product = 20;
        public const int Quotient = 25;

        public static readonly Dictionary<TokenType, OperationMetadata> Operators = new()
        {
            [TokenType.Assignment] = new(None, Operator.Assignment, Associativity.Right),
            [TokenType.Subtract] = new(Difference, Operator.Subtract, Associativity.Left),
            [TokenType.Plus] = new(Sum, Operator.Add, Associativity.Left),
            [TokenType.Divide] = new(Quotient, Operator.Divide, Associativity.Left),
            [TokenType.Multiply] = new(Product, Operator.Multiply, Associativity.Left),
        };

        public static OperationMetadata Get(TokenType type)
        {
            if (Operators.TryGetValue(type, out var metadata))
                return metadata;

            return new();
        }
    }
}
