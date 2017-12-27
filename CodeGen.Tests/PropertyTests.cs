using System.Linq;
using CodeGen.SimplifiedAst;
using Xunit;

namespace CodeGen.Tests
{
    public class PropertyTests
    {
        private readonly Class _parsedClass;

        public PropertyTests()
        {
            var source = new RoslynSource();

            var result = RoslynSource.Run("TestProject/TestProject.csproj");
            _parsedClass = result[0];
        }

        [Fact]
        public void Parses_public_property()
        {
            var property = _parsedClass.Properties.Single();
            Assert.Equal("AProperty", property.Name);
        }
    }
}