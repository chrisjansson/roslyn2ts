using System.Collections.Generic;
using System.Linq;
using CodeGen.SimplifiedAst;

namespace CodeGen.Abstract
{
    public class TypeFragment : IAbstractFragment
    {
        public TypeFragment(Class @class)
        {
            Type = @class;
        }

        public Class Type { get; }

        public IReadOnlyCollection<ITypeReference> GetDependencies()
        {
            return GetDependencies(Type)
                .ToList();
        }

        public IReadOnlyCollection<ITypeReference> GetExports()
        {
            return new List<ITypeReference>{ Type.TypeReference };
        }

        private List<ITypeReference> GetDependencies(Class @class)
        {
            var methodParameterTypes = @class.Methods
                .SelectMany(x => x.Parameters.Select(p => p.Type))
                .ToList();

            var returnTypes = @class.Methods
                .Select(x => x.ReturnType)
                .ToList();

            var propertyTypes = @class.Properties
                .Select(x => x.Type)
                .ToList();

            return methodParameterTypes
                .Concat(returnTypes)
                .Concat(propertyTypes)
                .ToList();
        }
    }
}