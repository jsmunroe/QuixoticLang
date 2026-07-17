using Quixotic.Interop;
using Quixotic.Interop.TypeSystem.Types;
using Quixotic.InteropTests.TestModels;

namespace Quixotic.InteropTests
{
    [TestClass]
    public sealed class ClrConverterTests
    {
        [TestMethod]
        public void Import_TestClassWithProperties()
        {
            // Setup
            var registry = new ClrTypeRegistry();
            var marshaller = new ClrMarshaller(registry);
            var importer = new TypeImporter(marshaller);

            // Execute
            var clrType = new ClrType(typeof(TestClassWithProperties));
            importer.Import(clrType);

            // Assert
        }
    }
}
