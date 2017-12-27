namespace CodeGen.SimplifiedAst
{
    public class CollectionTypeReference : ITypeReference
    {
        public CollectionTypeReference(ITypeReference innerType)
        {
            InnerType = innerType;
        }

        public ITypeReference InnerType { get; }

        public T Visit<T>(ITypeReferenceVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }
    }
}