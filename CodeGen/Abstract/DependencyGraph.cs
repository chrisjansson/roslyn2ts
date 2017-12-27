using System.Collections.Generic;
using CodeGen.SimplifiedAst;

namespace CodeGen.Abstract
{
    public class DependencyGraph
    {
        public DependencyGraph()
        {
            ModuleDependencies = new Dictionary<AbstractModule, List<(ClassTypeReference, AbstractModule)>>();
        }

        public Dictionary<AbstractModule, List<(ClassTypeReference, AbstractModule)>> ModuleDependencies { get; }
    }
}