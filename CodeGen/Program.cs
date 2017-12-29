using System;
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

            var controllers = result
                .Where(x => x.Name.EndsWith("Controller"))
                .Select(x => x.ReplaceTypeReference(new TaskVisitor()))
                .ToList();

            var controllerModules = new List<AbstractModule>();
            foreach (var controller in controllers)
            {
                var path = controller.Namespace.Split(".");
                var module = new AbstractModule(controller.Name.Replace("Controller", "Client"), path);
                var fragment = new ApiFragment();

                fragment.Parts.Add(controller);

                module.Fragments.Add(fragment);
                controllerModules.Add(module);
            }

            //todo: add other codegen dependencies here if there is a need
            var allModules = controllerModules.ToList();
            
            IReadOnlyCollection<ClassTypeReference> GetUnresolvedDependencies(IReadOnlyCollection<AbstractModule> modules)
            {
                var allNeededDependencies = modules
                    .SelectMany(x => x.GetDependencies())
                    .Distinct()
                    .ToHashSet();

                var allExportedTypes = modules
                    .SelectMany(x => x.GetExports())
                    .ToHashSet();

                var resolveTypesToImportvisitor =  new ResolveTypesToImportvisitor();
                
                return allNeededDependencies
                    .Except(allExportedTypes)
                    .SelectMany(x => resolveTypesToImportvisitor.Accept(x))
                    .ToList();
            }

            while (GetUnresolvedDependencies(allModules).Any())
            {
                //One class per module strategy, maybe others?
                foreach (var classDependency in GetUnresolvedDependencies(allModules))
                {
                    var @class = result.SingleOrDefault(x => Equals(x.TypeReference, classDependency));
                    if (@class == null)
                    {
                        //todo: show how to declare a type transform for this, also do the type transform..
                        throw new MissingTypeException(classDependency.Name, classDependency.Namespace);
                    }
                    var path = @class.Namespace.Split(".");
                    var module = new AbstractModule(@class.Name, path);
                    var fragment = new TypeFragment(@class);

                    module.Fragments.Add(fragment);
                    allModules.Add(module);
                }
            }

            //todo: validate that a single dependency is only in a single module?
            var dependencyGraph = new DependencyGraph();
            foreach (var module in allModules)
            {
                AbstractModule GetModuleForDependency(ITypeReference typeref)
                {
                    bool SatisfiesDependency(AbstractModule mod, ITypeReference type)
                    {
                        return mod.GetExports()
                            .Contains(type);
                    }

                    var moduleWithDependency = allModules
                        .Except(new[] {module})
                        .SingleOrDefault(x => SatisfiesDependency(x, typeref));

                    if (moduleWithDependency == null)
                    {
                        throw new Exception();
                        //throw new MissingTypeException(???);
                    }

                    return moduleWithDependency;
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

    public class MissingTypeException : Exception
    {
        public string Name { get; }
        public string Namespace { get; }

        public MissingTypeException(string name, string @namespace) : base(@namespace + "." + name)
        {
            Name = name;
            Namespace = @namespace;
        }
    }

    //todo: ClassTypeReference wrapped in IImportableType container if more than ClassTypeReference?
}