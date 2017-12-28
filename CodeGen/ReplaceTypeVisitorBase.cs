using CodeGen.SimplifiedAst;

namespace CodeGen
{
    public class ReplaceTypeVisitorBase : ITypeReferenceVisitor<ITypeReference>
    {
        public virtual ITypeReference Visit(PrimitiveTypeReference primitiveTypeReference)
        {
            return primitiveTypeReference;
        }

        public virtual ITypeReference Visit(CollectionTypeReference collectionTypeReference)
        {
            return collectionTypeReference;
        }

        public virtual ITypeReference Visit(ClassTypeReference classTypeReference)
        {
            return classTypeReference;
        }

        public virtual ITypeReference Visit(GenericClassTypeReference genericClassTypeReference)
        {
            return genericClassTypeReference;
        }
    }
}