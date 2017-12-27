using System.Collections.Generic;
using System.Linq;
using CodeGen.SimplifiedAst;

namespace CodeGen.Abstract
{
    public class AbstractModule
    {
        public AbstractModule(string name, string[] path)
        {
            Name = name;
            Path = path;
            Fragments = new List<IAbstractFragment>();
        }

        public string Name { get; }
        public string[] Path { get; }
        public List<IAbstractFragment> Fragments { get; }

        public IReadOnlyCollection<ITypeReference> GetDependencies()
        {
            return Fragments
                .SelectMany(x => x.GetDependencies())
                .Distinct()
                .ToList();
        }

        public IReadOnlyCollection<ITypeReference> GetExports()
        {
            return Fragments
                .SelectMany(x => x.GetExports())
                .Distinct()
                .ToList();
        }
    }
}