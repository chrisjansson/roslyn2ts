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
            var fragments = SerializeFragments(module.Fragments);
            return ConcatBlocks(imports, fragments);
        }

        private IReadOnlyCollection<string> SerializeFragments(List<IAbstractFragment> moduleFragments)
        {
            return moduleFragments
                .Select(x => SerializeFragment(x))
                .ToList();
        }

        private string SerializeFragment(IAbstractFragment abstractFragment)
        {
            switch (abstractFragment)
            {
                case TypeFragment typeFragment:
                    return SerializeTypeFragment(typeFragment);
                case ApiFragment apiFragment:
                    return SerializeApiFragment(apiFragment);
            }

            throw new NotImplementedException();
        }

        private string SerializeApiFragment(ApiFragment apiFragment)
        {
            var fetchAdapter = new string[]
            {
                "type FetchAdapter = {",
                Indent("get: <TParams, TReturn>(url: string, params?: TParams) => Promise<TReturn>;", 1),
                Indent("post: <TParams, TReturn>(url: string, params?: TParams) => Promise<TReturn>;", 1),
                Indent("put: <TParams, TReturn>(url: string, params?: TParams) => Promise<TReturn>;", 1),
                "}",
            };

            var parts = apiFragment.Parts
                .Select(x => SerializeApiFragmentPart(x))
                .ToList();

            return ConcatBlocks(fetchAdapter, parts);
        }

        private string SerializeApiFragmentPart(Class @class)
        {
            var methodLines = @class.Methods
                .Select(x => Indent(SerializeMethod(x), 1))
                .ToArray();

            var methodConcats = ConcatBlocks(methodLines);
            
            var lines = new string[]
            {
                $"export class {@class.Name} {{",
                Indent("constructor(private readonly client: FetchAdapter) {}", 1),
                "",
                methodConcats,
                $"}}",
            };

            return string.Join(Environment.NewLine, lines);
        }

        private class IsVoidVisitor : ITypeReferenceVisitor<bool>
        {
            public bool Visit(PrimitiveTypeReference primitiveTypeReference)
            {
                return primitiveTypeReference.Kind == PrimitiveKind.Void;
            }

            public bool Visit(CollectionTypeReference collectionTypeReference)
            {
                return false;
            }

            public bool Visit(ClassTypeReference classTypeReference)
            {
                return false;
            }

            public bool Visit(GenericClassTypeReference genericClassTypeReference)
            {
                return false;
            }
        }

        private IReadOnlyCollection<string> SerializeMethod(Method method)
        {
            var isVoid = method.ReturnType.Visit(new IsVoidVisitor());
            var verb = isVoid ? "post" : "get";
            var url = "\"someUrl\"";

            //only single parameter methods are supported as of now
            var parameter = method.Parameters.SingleOrDefault();
            var returnType = TypescriptModel.TranslateType(method.ReturnType);

            var methodParameterSignature = parameter != null ? $"{ToPascalCase(parameter.Name)}: {TypescriptModel.TranslateType(parameter.Type)}" : "";

            var parameterList = parameter != null ? $", {ToPascalCase(parameter.Name)}" : "";
            
            var lines = new string[]
            {
                $"{ToPascalCase(method.Name)} = ({methodParameterSignature}): Promise<{returnType}> => {{",
                Indent($"return this.client.{verb}({url}{parameterList});", 1),
                $"}}",
            };

            return lines;
        }

        private string SerializeTypeFragment(TypeFragment typeFragment)
        {
            var type = typeFragment.Type;

            var propertyDeclarations = type.Properties
                .Select(x => Indent(SerializeProperty(x), 1))
                .ToList();

            var lines = new[]
            {
                $"export interface {type.Name} {{",
                string.Join(Environment.NewLine, propertyDeclarations),
                $"}}"
            };

            return string.Join(Environment.NewLine, lines);
        }

        private string Indent(string s, int n)
        {
            return string.Join("",Enumerable.Repeat(" ", n * 4)) + s;
        }
        
        private IReadOnlyCollection<string> Indent(IEnumerable<string> s, int n)
        {
            return s
                .Select(x => Indent(x, n))
                .ToList();
        }

        private string SerializeProperty(Property property)
        {
            return $"{ToPascalCase(property.Name)}: {TypescriptModel.TranslateType(property.Type)};";
        }

        private string ToPascalCase(string s)
        {
            return s.Substring(0, 1).ToLower() + s.Substring(1);
        }

        private string ConcatBlocks(params IReadOnlyCollection<string>[] blocks)
        {
            var joinedBlocks = blocks
                .Select(block => string.Join(Environment.NewLine, block))
                .Where(x => !string.IsNullOrEmpty(x))
                .ToList();

            return string.Join(Environment.NewLine + Environment.NewLine, joinedBlocks);
        }

        private IReadOnlyCollection<string> SerializeImports(AbstractModule module, List<(ClassTypeReference, AbstractModule)> moduleDependencies)
        {
            return moduleDependencies
                .Select(x => SerializeImport(module, x))
                .ToList();
        }

        private string SerializeImport(AbstractModule module, (ClassTypeReference, AbstractModule) valueTuple)
        {
            var importPath = GetImportPathBetween(module, valueTuple.Item2);
            return $"import {{ {valueTuple.Item1.Name} }} from \"./{importPath}\";";
        }

        public static string GetImportPathBetween(AbstractModule currentModule, AbstractModule module)
        {
            var p0 = currentModule.Path
                .Zip(module.Path, (l, r) => (l, r))
                .SkipWhile(t => t.Item1 == t.Item2)
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