using System.Collections.Generic;
using System.Linq;
using CodeGen.Abstract;
using CodeGen.SimplifiedAst;

namespace CodeGen
{
    public class ApiFragment : IAbstractFragment
    {
        public ApiFragment()
        {
            Parts = new List<Class>();
        }
        
        public IReadOnlyCollection<ITypeReference> GetDependencies()
        {
            return Parts
                .SelectMany(x => GetClassReferences(x))
                .ToList();
        }

        private IReadOnlyCollection<ITypeReference> GetClassReferences(Class @class)
        {
            var parameterTypes = @class.Methods
                .SelectMany(x => x.Parameters.Select(p => p.Type));

            var returnTypes = @class.Methods
                .Select(x => x.ReturnType);

            return parameterTypes.Concat(returnTypes)
                .ToList();
        }

        public IReadOnlyCollection<ITypeReference> GetExports()
        {
            return new List<ITypeReference>();
        }

        public List<Class> Parts { get; }
    }
}