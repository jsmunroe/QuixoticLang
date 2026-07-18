using Quixotic.Common.TypeSystem.Types;
using Quixotic.Interop;
using Quixotic.Interop.Modules;

namespace Quixotic.Standard.Math
{
    public class MathModule() : Module("Quixotic.Standard.Math")
    {
        protected override IEnumerable<QxType> GetTypes()
        {
            var typeLoader = ClrTypeLoader.FromTypes(typeof(Complex));

            return typeLoader.Load();
        }
    }
}
