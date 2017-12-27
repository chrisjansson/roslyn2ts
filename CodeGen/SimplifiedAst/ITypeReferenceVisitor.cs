namespace CodeGen.SimplifiedAst
{
    public interface ITypeReferenceVisitor<out T>
    {
        T Visit(PrimitiveTypeReference primitiveTypeReference);
        T Visit(CollectionTypeReference collectionTypeReference);
        T Visit(ClassTypeReference classTypeReference);
    }
}