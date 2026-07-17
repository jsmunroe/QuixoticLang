using Quixotic.Interop;
using Quixotic.Standard.Math;

namespace Quixotic.Standard.MathTests
{
    [TestClass]
    public sealed class ComplexTests
    {
        [TestMethod]
        public void Import_Complex()
        {
            // Setup
            var registry = new ClrTypeRegistry();
            var marshaller = new ClrMarshaller(registry);

            // Execute
            var clrType = marshaller.Wrap(typeof(Complex));

            // Assert
            Assert.IsNotNull(clrType);
        }
    }
}
