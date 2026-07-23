using Quixotic.Common.Environment;
using Quixotic.Common.TypeSystem.Types;

namespace Quixotic.CommonTests.Environment
{
    [TestClass]
    public class TypeRegistryTests
    {
        [TestMethod]
        public void Resolve_simple_types()
        {
            // Setup
            var registry = new TypeRegistry();
            registry.Register("number", QxType.Number);

            // Execute
            var result = registry.Resolve("number");

            // Assert
            Assert.IsNotNull(result);
            Assert.AreSame(QxType.Number, result);
        }

        [TestMethod]
        public void Resolve_generic_array_types()
        {
            // Setup
            var registry = new TypeRegistry();
            registry.Register("number", QxType.Number);
            registry.Register("{TItem}[]", QxType.Array);

            // Execute
            var result = registry.Resolve("number[]");

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(QxType.Array.MakeGenericType(QxType.Number), result);
        }


    }
}
