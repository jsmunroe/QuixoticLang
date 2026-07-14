using Quixotic.Common.Exceptions.Interpret;
using System.ComponentModel;

namespace Quixotic.Common.TypeSystem.Types
{

    [Description("set")]
    public class SetType(QxType elementType) : CollectionType($"set<{elementType}>", elementType)
    {
        public Instance Construct(Instance[] elements)
        {
            var set = new Instance(this);

            Assign(set, elements);

            return set;
        }

        public override CollectionType Create(QxType elementType)
        {
            return new ArrayType(elementType);
        }

        public void Assign(Instance set, Instance[] elements)
        {
            if (!set.Type.Equals(this))
                throw new InvalidOperationException($"Instance is not of type {this}.");

            foreach (var element in elements)
            {
                if (!ElementType.IsAssignableFrom(element.Type))
                    throw new VariableTypeMismatchException(element.Type, ElementType);
            }

            set["elements"] = (Instance[])[.. elements];
        }

        public override bool IsAssignableFrom(QxType other)
        {
            if (other is not SetType setType)
                return false;

            if (!ElementType.IsAssignableFrom(setType.ElementType))
                return false;

            return true;
        }

        protected override Instance[] CleanElements(Instance[] elements)
        {
            return [.. elements.Distinct().OrderBy(e => Random.Shared.Next())];
        }

        public override string ToString(Instance instance)
        {
            var elements = (Instance[])instance["elements"]!;

            return $"{{{string.Join(", ", elements.Select(e => e.ToString()))}}}";
        }
    }
}
