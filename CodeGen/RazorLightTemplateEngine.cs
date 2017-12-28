using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CodeGen.Abstract;
using CodeGen.SimplifiedAst;
using RazorLight;

namespace CodeGen
{
    public class RazorLightTemplateEngineFactory
    {
        public RazorLightEngine Create(string directory)
        {
            return new RazorLightEngineBuilder()
                .UseFilesystemProject(directory)
                .UseMemoryCachingProvider()
                .Build();
        }
    }

    public class TypescriptModel
    {
        public AbstractModule CurrentModule { get; set; }
        public List<(ClassTypeReference, AbstractModule)> Dependencies { get; set; }

        public static string TranslateType(ITypeReference propertyType)
        {
            var translator = new TypescriptTypeReferenceTranslator();
            return translator.Accept(propertyType);
        }

        public string GetPartial(IAbstractFragment fragment)
        {
            switch (fragment)
            {
                case TypeFragment typeFragment:
                    return "TypeFragment.cshtml";
                case ApiFragment apiFragment:
                    return "ApiFragment.cshtml";
            }

            throw new NotImplementedException();
        }
    }
}
