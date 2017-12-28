using System.Linq;
using CodeGen.SimplifiedAst;

namespace CodeGen
{
    public static class ClassExtensions
    {
        public static Class ReplaceTypeReference(this Class @class, ITypeReferenceVisitor<ITypeReference> visitor)
        {
            var properties = @class.Properties
                .Select(x => new Property
                {
                    Name = x.Name,
                    Type = x.Type.Visit(visitor)
                })
                .ToList();

            var methods = @class.Methods
                .Select(x =>
                    new Method
                    {
                        Name = x.Name,
                        Parameters = x.Parameters
                            .Select(p => new Parameter
                            {
                                Name = p.Name,
                                Type = p.Type.Visit(visitor)
                            })
                            .ToList(),
                        ReturnType = x.ReturnType.Visit(visitor)
                    }
                )
                .ToList();

            return new Class
            {
                Name = @class.Name,
                Methods = methods,
                Properties = properties,
                Namespace = @class.Namespace
            };
        }
    }
}