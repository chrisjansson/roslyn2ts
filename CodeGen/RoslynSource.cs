using System.Collections.Generic;
using System.Linq;
using Buildalyzer;
using Buildalyzer.Workspaces;
using CodeGen.SimplifiedAst;

namespace CodeGen
{
    public class RoslynSource
    {
        public static List<Class> Run(string filePath)
        {
            var manager = new AnalyzerManager();
            var analyzer = manager.GetProject(filePath);
            var workspace = analyzer.GetWorkspace();

            var compilation = workspace.CurrentSolution
                .Projects.First()
                .GetCompilationAsync().Result;

            var walkResult = new List<Class>();
            foreach (var project in workspace.CurrentSolution.Projects)
            foreach (var document in project.Documents)
            {
                var syntaxTree = document.GetSyntaxTreeAsync().Result;
                var model = compilation.GetSemanticModel(syntaxTree);
                var walker = new ClassCollector(model);
                walker.Visit(syntaxTree.GetRoot());
                walkResult.AddRange(walker.Result);
            }

            return walkResult;
        }
    }
}