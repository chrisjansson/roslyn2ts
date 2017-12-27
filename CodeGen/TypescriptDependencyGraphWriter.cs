using System.Collections.Generic;
using System.Threading.Tasks;
using CodeGen.Abstract;
using CodeGen.SimplifiedAst;

namespace CodeGen
{
    public class TypescriptDependencyGraphWriter
    {
        public async Task<List<File>> WriteDependencyGraph(DependencyGraph graph)
        {
            var files = new List<File>();
            foreach (var moduleDependency in graph.ModuleDependencies)
            {
                var module = moduleDependency.Key;
                var moduleDependencies = moduleDependency.Value;
                var file = await WriteModuleToFile(module, moduleDependencies);
                files.Add(file);
            }
            return files;
        }

        private async Task<File> WriteModuleToFile(AbstractModule module, List<(ClassTypeReference, AbstractModule)> moduleDependencies)
        {
            var moduleSerializer = new TypescriptModuleSerializer();
            var result = await moduleSerializer.SerializeModule(module, moduleDependencies);
            return new File
            {
                Path = module.Path,
                Name = module.Name + ".ts",
                Content = result
            };
        }
    }
}