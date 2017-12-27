using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CodeGen.Abstract;
using CodeGen.SimplifiedAst;

namespace CodeGen
{
    public class TypescriptModuleSerializer
    {
        public async Task<string> SerializeModule(AbstractModule module, List<(ClassTypeReference, AbstractModule)> moduleDependencies)
        {
            var f = new RazorLightTemplateEngineFactory();
            var engine = f.Create("C:\\code\\codegen\\codegen\\");
            return await engine.CompileRenderAsync("Type.cshtml", new TypescriptModel
            {
                CurrentModule = module,
                Dependencies = moduleDependencies
            });
        }

        public static string GetImportPathBetween(AbstractModule currentModule, AbstractModule module)
        {
            var p0 = currentModule.Path
                .Zip(module.Path, (l, r) => (l, r))
                .SkipWhile(((t) => t.Item1 == t.Item2))
                .ToList();

            var lengthSame = currentModule.Path.Length - p0.Count;

            var backToRoot = Enumerable.Repeat("..", p0.Count);

            var downPath = module.Path
                .Skip(lengthSame)
                .ToList();

            var fullPath = backToRoot
                .Concat(downPath)
                .Concat(new[] {module.Name});

            return string.Join("/", fullPath);
        }
    }
}