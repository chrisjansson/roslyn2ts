using System.Linq;
using CodeGen.SimplifiedAst;
using Xunit;

namespace CodeGen.Tests
{
    public class MethodTests
    {
        private readonly Class _parsedClass;

        public MethodTests()
        {
            var source = new RoslynSource();

            var result = RoslynSource.Run("TestProject/TestProject.csproj");
            _parsedClass = result[0];
        }

        [Fact]
        public void Parses_parameterless_void_method()
        {
            var method = Method("A");
            Assert.Equal(0, method.Parameters.Count);
            Assert.Equal(PrimitiveKind.Void, ((PrimitiveTypeReference)method.ReturnType).Kind);
        }
        
        [Fact]
        public void Parses_parameterless_int_method()
        {
            var method = Method("Int");
            Assert.Equal(0, method.Parameters.Count);
            Assert.Equal(PrimitiveKind.Int32, ((PrimitiveTypeReference)method.ReturnType).Kind);
        }
        
        [Fact]
        public void Parses_parameters()
        {
            var method = Method("Parameters");
            Assert.Equal(4, method.Parameters.Count);
            var p0 = method.Parameters[0];
            var p1 = method.Parameters[1];
            var p2 = method.Parameters[2];
            var p3 = method.Parameters[3];
            
            Assert.Equal("parameter1", p0.Name);
            Assert.Equal(PrimitiveKind.Int32, ((PrimitiveTypeReference)p0.Type).Kind);
            Assert.Equal("parameter2", p1.Name);
            Assert.Equal(PrimitiveKind.String, ((PrimitiveTypeReference)p1.Type).Kind);
            Assert.Equal("parameter3", p2.Name);
            Assert.Equal(PrimitiveKind.Double, ((PrimitiveTypeReference)p2.Type).Kind);
            Assert.Equal(PrimitiveKind.Decimal, ((PrimitiveTypeReference)p3.Type).Kind);
        }

        [Fact]
        public void Parses_list_parameter()
        {
            var method = Method("Lists");
            var p0 = method.Parameters[0];
            var innerType = ((CollectionTypeReference) p0.Type).InnerType;
            Assert.Equal(PrimitiveKind.Int32, ((PrimitiveTypeReference)innerType).Kind);
        }

        [Fact]
        public void Ignores_private_method()
        {
            Assert.DoesNotContain(_parsedClass.Methods, x => x.Name == "B");
        }

        private Method Method(string name)
        {
            var method = _parsedClass.Methods.SingleOrDefault(x => x.Name == name);
            Assert.NotNull(method);
            return method;
        }
    }
}