using System.Collections.Generic;

namespace CodeGen.SimplifiedAst
{
    public class Method
    {
        public string Name { get; set; }
        public List<Parameter> Parameters { get; set; }
        public ITypeReference ReturnType { get; set; }
    }
}