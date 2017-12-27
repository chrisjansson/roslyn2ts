using System.Collections.Generic;
using CodeGen.Abstract;
using CodeGen.SimplifiedAst;

namespace CodeGen
{
    public class TypescriptDependencyGraphWriter
    {
        public List<File> WriteDependencyGraph(DependencyGraph graph)
        {
            var files = new List<File>();
            foreach (var moduleDependency in graph.ModuleDependencies)
            {
                var module = moduleDependency.Key;
                var moduleDependencies = moduleDependency.Value;
                var file = WriteModuleToFile(module, moduleDependencies);
                files.Add(file);
            }
            return files;
        }

        private File WriteModuleToFile(AbstractModule module, List<(ClassTypeReference, AbstractModule)> moduleDependencies)
        {
            var moduleSerializer = new TypescriptModuleSerializer();
            var result = moduleSerializer.SerializeModule(module, moduleDependencies);
            return new File
            {
                Path = module.Path,
                Name = module.Name + ".ts",
                Content = result
            };
        }
    }
}