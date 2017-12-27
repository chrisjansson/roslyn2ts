using System.Collections.Generic;
using CodeGen.SimplifiedAst;

namespace CodeGen
{
    public class ResolveTypesToImportvisitor : ITypeReferenceVisitor<IReadOnlyCollection<ClassTypeReference>>
    {
        public IReadOnlyCollection<ClassTypeReference> Visit(PrimitiveTypeReference primitiveTypeReference)
        {
            return new List<ClassTypeReference>();
        }

        public IReadOnlyCollection<ClassTypeReference> Visit(CollectionTypeReference collectionTypeReference)
        {
            return collectionTypeReference.InnerType.Visit(this);
        }

        public IReadOnlyCollection<ClassTypeReference> Visit(ClassTypeReference classTypeReference)
        {
            return new List<ClassTypeReference> {classTypeReference};
        }

        public IReadOnlyCollection<ClassTypeReference> Accept(ITypeReference typeReference)
        {
            return typeReference.Visit(this);
        }
    }
}