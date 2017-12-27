using CodeGen.SimplifiedAst;

namespace CodeGen.Abstract
{
    public class RequiresImportVisitor : ITypeReferenceVisitor<bool>
    {
        public bool Visit(PrimitiveTypeReference primitiveTypeReference)
        {
            return false;
        }

        public bool Visit(CollectionTypeReference collectionTypeReference)
        {
            return false;
        }

        public bool Visit(ClassTypeReference classTypeReference)
        {
            return true;
        }

        public bool Accept(ITypeReference typeReference)
        {
            return typeReference.Visit(this);
        }
    }
}