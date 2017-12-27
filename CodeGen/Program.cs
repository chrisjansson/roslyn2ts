using System.Collections.Generic;
using System.IO;
using System.Linq;
using CodeGen.Abstract;
using CodeGen.SimplifiedAst;

namespace CodeGen
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var result = RoslynSource.Run(args[0]);

            var modules = new List<AbstractModule>();
            foreach (var @class in result)
            {
                var path = @class.Namespace.Split(".");
                var module = new AbstractModule(@class.Name, path);
                var fragment = new TypeFragment();

                fragment.Types.Add(@class);

                module.Fragments.Add(fragment);
                modules.Add(module);
            }

            //todo: validate that a single dependency is only in a single module?
            var dependencyGraph = new DependencyGraph();
            foreach (var module in modules)
            {
                AbstractModule GetModuleForDependency(ITypeReference typeref)
                {
                    bool SatisfiesDependency(AbstractModule mod, ITypeReference type)
                    {
                        return mod.GetExports()
                            .Contains(type);
                    }

                    return modules
                        .Except(new[] {module})
                        .Single(x => SatisfiesDependency(x, typeref));
                }

                var resolveTypesToImportvisitor =  new ResolveTypesToImportvisitor();
                var dependencyRelationships = module
                    .GetDependencies()
                    .SelectMany(x => resolveTypesToImportvisitor.Accept(x))
                    .Select(x => (x, GetModuleForDependency(x)))
                    .ToList();

                dependencyGraph.ModuleDependencies[module] = dependencyRelationships;
            }
            var writer = new TypescriptDependencyGraphWriter();

            var files = writer.WriteDependencyGraph(dependencyGraph);

            Directory.Delete("C:\\temp\\ts", true);
            Directory.CreateDirectory("C:\\temp\\ts");

            foreach (var file in files)
            {
                var folderPath = Path.Combine(new[] {"C:\\temp\\ts"}.Concat(file.Path).ToArray());
                Directory.CreateDirectory(folderPath);
                var filePath = Path.Combine(folderPath, file.Name);
                System.IO.File.WriteAllText(filePath, file.Content);
            }
        }
    }

    //todo: ClassTypeReference wrapped in IImportableType container if more than ClassTypeReference?
}