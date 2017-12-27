using System.Collections.Generic;
using System.Linq;
using CodeGen.SimplifiedAst;

namespace CodeGen.Abstract
{
    public class TypeFragment : IAbstractFragment
    {
        public TypeFragment()
        {
            Types = new List<Class>();
        }

        public List<Class> Types { get; }

        public IReadOnlyCollection<ITypeReference> GetDependencies()
        {
            return Types
                .SelectMany(GetDependencies)
                .ToList();
        }

        public IReadOnlyCollection<ITypeReference> GetExports()
        {
            return Types
                .Select(x => x.TypeReference)
                .Distinct()
                .ToList();
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