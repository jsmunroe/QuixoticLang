using Quixotic.Common.TypeSystem.Types;

namespace Quixotic.Common.TypeSystem
{
    public class TypedInstanceView(Instance instance, QxType asType) : Instance(asType, instance)
    {
        private readonly Instance _instance = instance;
    }
}
