namespace CodeGen.SimplifiedAst
{
    public interface ITypeReference
    {
        T Visit<T>(ITypeReferenceVisitor<T> visitor);
    }
}