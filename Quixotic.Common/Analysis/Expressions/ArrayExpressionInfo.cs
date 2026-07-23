using Quixotic.Common.Expressions;
using Quixotic.Common.TypeSystem.Types;

namespace Quixotic.Common.Analysis.Expressions
{
    public class ArrayExpressionInfo(QxType elementType, QxExpression expression) : CollectionExpressionInfo(elementType, QxType.Array.MakeGenericType(elementType), expression)
    { }
}
