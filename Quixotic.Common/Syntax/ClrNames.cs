using System.Diagnostics.CodeAnalysis;

namespace Quixotic.Common.Syntax
{
    public class ClrNames
    {
        public static ClrNames Current { get; } = new();

        private ClrNames()
        { }

        private readonly Dictionary<string, string> _names = new()
        {
            ["op_UnaryPlus"] = "+",
            ["op_UnaryNegation"] = "-",
            ["op_LogicalNot"] = "not",
            ["op_OnesComplement"] = "~",
            ["op_Increment"] = "++",
            ["op_Decrement"] = "--",
            ["op_Addition"] = "+",
            ["op_Subtraction"] = "-",
            ["op_Multiply"] = "*",
            ["op_Division"] = "/",
            ["op_Modulus"] = "%",
            ["op_BitwiseAnd"] = "&",
            ["op_BitwiseOr"] = "|",
            ["op_ExclusiveOr"] = "^",
            ["op_LeftShift"] = "<<",
            ["op_RightShift"] = ">>",
            ["op_Equality"] = "==",
            ["op_Inequality"] = "!=",
            ["op_LessThan"] = "<",
            ["op_LessThanOrEqual"] = "<=",
            ["op_GreaterThan"] = ">",
            ["op_GreaterThanOrEqual"] = ">=",
        };

        public bool TryResolve(string name, [NotNullWhen(returnValue: true)] out string value)
        {
            return _names.TryGetValue(name, out value!);
        }
    }
}
