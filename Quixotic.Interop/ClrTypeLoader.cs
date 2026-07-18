using Quixotic.Interop.Attributes;
using Quixotic.Interop.TypeSystem.Types;
using System.Reflection;

namespace Quixotic.Interop
{
    public class ClrTypeLoader(IEnumerable<Type> types)
    {
        public IEnumerable<ClrType> Load()
        {
            var registry = new ClrTypeRegistry();
            var marshaller = new ClrMarshaller(registry);

            foreach (var type in types)
            {
                var clrType = (ClrType)marshaller.Wrap(type);
                yield return clrType;
            }
        }

        public static ClrTypeLoader FromAssembly(Assembly assembly)
        {
            var types = assembly.GetTypesWithAttribute<QuixoticClrTypeAttribute>();

            return new(types);
        }

        public static ClrTypeLoader FromTypes(params Type[] types)
        {
            return new(types);
        }
    }
}
