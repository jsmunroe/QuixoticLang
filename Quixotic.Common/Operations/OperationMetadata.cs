using Quixotic.Common.Tokens;

namespace Quixotic.Common.Operations
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
        public const int Indexer = 30;
        public const int Dot = 30;

        private static Dictionary<(OperationType, TokenType), OperationMetadata> Operators { get; } = new()
        {
            [(OperationType.Binary, TokenType.Assignment)] = new(Assign, ":=", Operator.Assignment, Associativity.Right),
            [(OperationType.Binary, TokenType.Subtract)] = new(Difference, "-", Operator.Subtract, Associativity.Left),
            [(OperationType.Binary, TokenType.Plus)] = new(Sum, "+", Operator.Add, Associativity.Left),
            [(OperationType.Binary, TokenType.Divide)] = new(Quotient, "/", Operator.Divide, Associativity.Left),
            [(OperationType.Binary, TokenType.Multiply)] = new(Product, "*", Operator.Multiply, Associativity.Left),
            [(OperationType.Binary, TokenType.EqualTo)] = new(Comparison, "=", Operator.EqualTo, Associativity.Left),
            [(OperationType.Binary, TokenType.NotEqualTo)] = new(Comparison, "!=", Operator.NotEqualTo, Associativity.Left),
            [(OperationType.Binary, TokenType.LessThan)] = new(Comparison, "<", Operator.LessThan, Associativity.Left),
            [(OperationType.Binary, TokenType.LessThanOrEqualTo)] = new(Comparison, "<=", Operator.LessThanOrEqualTo, Associativity.Left),
            [(OperationType.Binary, TokenType.GreaterThan)] = new(Comparison, ">", Operator.GreaterThan, Associativity.Left),
            [(OperationType.Binary, TokenType.GreaterThanOrEqualTo)] = new(Comparison, ">=", Operator.GreaterThanOrEqualTo, Associativity.Left),
            [(OperationType.Binary, TokenType.In)] = new(Comparison, "in", Operator.In, Associativity.Left),
            [(OperationType.Binary, TokenType.And)] = new(Logical, "and", Operator.And, Associativity.Left),
            [(OperationType.Binary, TokenType.Or)] = new(Logical, "or", Operator.Or, Associativity.Left),
            [(OperationType.Binary, TokenType.OpenBracket)] = new(Indexer, "[", Operator.Indexer, Associativity.Left),
            [(OperationType.Binary, TokenType.Dot)] = new(Dot, ".", Operator.Dot, Associativity.Left),
            [(OperationType.Unary, TokenType.Not)] = new(Logical, "not", Operator.Not, Associativity.Right),
            [(OperationType.Unary, TokenType.Subtract)] = new(Difference, "-", Operator.Subtract, Associativity.Right),
            [(OperationType.Unary, TokenType.Plus)] = new(Sum, "+", Operator.Add, Associativity.Right),
        };

        private static HashSet<TokenType> OperatorTokens { get; } = [.. Operators.Keys.Select(k => k.Item2)];


        public static OperationMetadata Get(OperationType operationType, TokenType type)
        {
            if (Operators.TryGetValue((operationType, type), out var metadata))
                return metadata;

            return new();
        }

        public static OperationMetadata GetBinary(TokenType type) => Get(OperationType.Binary, type);

        public static OperationMetadata GetUnary(TokenType type) => Get(OperationType.Unary, type);

        public static bool IsOperator(TokenType type) => OperatorTokens.Contains(type);

        public static string? GetOperatorValue(Operator @operator)
        {
            var metadata = Operators.Values.FirstOrDefault(m => m.Operator == @operator);

            return metadata?.Value;
        }
    }
}
