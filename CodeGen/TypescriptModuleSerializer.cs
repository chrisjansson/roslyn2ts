using System;
using System.Collections.Generic;
using System.Linq;
using CodeGen.Abstract;
using CodeGen.SimplifiedAst;

namespace CodeGen
{
    public class TypescriptModuleSerializer
    {
        public string SerializeModule(AbstractModule module, List<(ClassTypeReference, AbstractModule)> moduleDependencies)
        {
            var imports = SerializeImports(module, moduleDependencies);

            var fragments = module.Fragments
                .Select(x => SerializeFragment(x))
                .ToList();

            var joinedFragments = string.Join(Environment.NewLine, fragments);

            return imports + Environment.NewLine + joinedFragments;
        }

        private string SerializeImports(AbstractModule currentModule, List<(ClassTypeReference, AbstractModule)> moduleDependencies)
        {
            var imports = moduleDependencies
                .Select(x => SerializeImport(currentModule, x))
                .ToList();

            return string.Join(Environment.NewLine, imports);
        }

        private string SerializeFragment(IAbstractFragment abstractFragment)
        {
            var typeFragment = (TypeFragment) abstractFragment;
            var serializedTypes = typeFragment.Types
                .Select(x => SerializeType(x))
                .ToList();

            return string.Join(Environment.NewLine, serializedTypes);
        }

        private string SerializeType(Class @class)
        {
            var properties = @class.Properties
                .Select(x => Indent(SerializeField(x), 1))
                .ToList();

            var joinedProperties = string.Join(Environment.NewLine, properties);

            return $@"export interface {@class.Name} {{
{joinedProperties}
}}";
        }

        private string Indent(string line, int times)
        {
            var indentation = "    ";
            var fullIndentation = string.Join(string.Empty, Enumerable.Repeat(indentation, times));

            return fullIndentation + line;
        }

        private string SerializeField(Property property)
        {
            var translator = new TypescriptTypeReferenceTranslator();
            var tsType = translator.Accept(property.Type);
            return $"{property.Name}: {tsType};";
        }

        private string SerializeImport(AbstractModule currentModule, (ClassTypeReference, AbstractModule) dependency)
        {
            var fullPath = GetImportPathBetween(currentModule, dependency);
            var path = string.Join("/", fullPath);
            return $"import {{ {dependency.Item1.Name} }} from \"./{path}\";";
        }

        private static IEnumerable<string> GetImportPathBetween(AbstractModule currentModule, (ClassTypeReference, AbstractModule) dependency)
        {
            var p0 = currentModule.Path
                .Zip(dependency.Item2.Path, (l, r) => (l, r))
                .SkipWhile(((t) => t.Item1 == t.Item2))
                .ToList();

            var lengthSame = currentModule.Path.Length - p0.Count;

            var backToRoot = Enumerable.Repeat("..", p0.Count);

            var downPath = dependency.Item2.Path
                .Skip(lengthSame)
                .ToList();

            return backToRoot
                .Concat(downPath)
                .Concat(new[] {dependency.Item2.Name});
        }
    }
}