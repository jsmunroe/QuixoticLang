using Quixotic.Common.Expressions;
using Quixotic.Common.TypeSystem.Types;

namespace Quixotic.Common.Analysis.Expressions
{
    public class VoidExpressionInfo(QxExpression expression) : ExpressionInfo(QxType.Void.Type, expression)
    { }
}
